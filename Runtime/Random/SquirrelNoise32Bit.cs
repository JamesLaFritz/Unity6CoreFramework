#region Header
// SquirrelNoise.cs
// Author:
//         Original C++ Squirrel Eiserloh
//         Translated from C++ by James LaFritz
// Description: A noise-based pseudo-random number generator inspired by SMU Guildhall's Squirrel Eiserloh's
// 2017 GDC Math for Game Programmers talk Noise-Based RNG https://www.youtube.com/watch?v=LWFzPP8ZbdU
// RNGs vs. noise functions, and shows how the latter can replace the former in your math library
//  and provide many other benefits
//  (unordered access, better reseeding, record/playback, network loss tolerance, lock-free parallelization, etc.)
//  while being smaller, faster, and easier to use.
// Also added GetUInt64
#endregion

using System.Runtime.CompilerServices;
using CoreFramework.Mathematics;
using Unity.Burst;
using Unity.Mathematics;
using static CoreFramework.Random.HashBasedNoiseUtils;

namespace CoreFramework.Random
{
    /// <summary>
    /// Provides stateless, deterministic 32-bit noise using integer indexing and optional seeding.
    /// </summary>
    [BurstCompile]
    public static class SquirrelNoise32Bit
    {
        #region Constants

        /// <summary>
        /// Represents the default noise type used in the SquirrelNoise32Bit class for noise generation methods.
        /// The default value is <c>NoiseType.MangledBitsBalancedMix</c>, which provides balanced mixing of bits for noise generation.
        /// </summary>
        private const NoiseType DefaultNoiseType = NoiseType.MangledBitsBalancedMix; // Default noise type to use

        #endregion

        // ToDo: Add ulong4(Get256)
        // ToDo: possible ulong8(GetUint512), and ulong16(GetUint1024)

        /// <summary>
        /// Computes a 128-bit unsigned integer by combining two 64-bit pseudo-random numbers derived from the input parameters.
        /// This method generates low and high 64-bit components based on a deterministic noise function.
        /// </summary>
        /// <param name="index">
        /// A signed integer used to indicate the position or index for generating pseudo-random noise.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer used as an additional input to control the randomness of the output.
        /// </param>
        /// <param name="type">
        /// The noise generation type that determines the method used for computing the noise values.
        /// Optional, defaults to a predefined type if not provided.
        /// </param>
        /// <returns>
        /// An instance of <c>ulong2</c> representing a 128-bit unsigned integer, composed of two 64-bit pseudo-random values.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 GetUInt128(int index, uint seed, NoiseType type = DefaultNoiseType)
        {
            var lo64 = GetUInt64(index, seed, type);

            // Fold both halves of lo64 to avoid throwing away entropy
            var lo32 = (uint)lo64;
            var hi32 = (uint)(lo64 >> 32);
            var fold = lo32 ^ (hi32 * GoldenRatio);

            // Derive a decorrelated index for the high half; keep math unchecked
            var hiIndex = unchecked(index ^ (int)fold);

            // Optional extra decorrelation: use a mixed seed for the second draw
            // var hiSeed = HashBasedNoiseUtils.FBmMixOctave(seed, 1); // if you expose a MixOctave wrapper
            // var hi64  = GetUInt64(hiIndex, hiSeed, type);
            var hi64 = GetUInt64(hiIndex, seed, type);

            return new ulong2(hi64, lo64);
        }

        /// <summary>
        /// Generates a 64-bit unsigned pseudo-random integer by combining two noise values.
        /// This method uses a deterministic noise function derived from the provided parameters.
        /// </summary>
        /// <param name="index">
        /// A signed integer representing the index or position used for generating the pseudo-random value.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer serving as a secondary input to influence the randomness of the output.
        /// </param>
        /// <param name="type">
        /// The noise generation algorithm type that specifies the method for producing the noise values.
        /// If not specified, a default noise type is used.
        /// </param>
        /// <returns>
        /// A 64-bit unsigned integer combining two noise components (high and low) to produce a unified pseudo-random value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetUInt64(int index, uint seed, NoiseType type = DefaultNoiseType)
        {
            var lo = Get1DNoise(index, seed, type);
            var hi = Get2DNoise(index, (int)lo, seed, type);
            return ((ulong)hi << 32) | lo;
        }

        #region 1D/2D/3D/4D Noise (UInt32)

        /// <summary>
        /// Returns a 32-bit unsigned integer containing reasonably well-scrambled bits, based on a given
        /// (signed) integer input parameter (position/index) and [optional] seed. Kind of like looking
        /// up a value in an infinitely large [non-existent] table of previously generated random numbers.
        ///
        /// The base bit-noise constants were crafted to have distinctive and interesting bits
        ///  and have so far produced excellent experimental test results.
        /// </summary>
        /// <remarks>
        /// Squirrel Eiserloh call this particular approach SquirrelNoise, specifically SquirrelNoise3 (version 3).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get1DNoise(int index, uint seed, NoiseType type = DefaultNoiseType)
        {
            return type switch
            {
                NoiseType.MangledBits => MangledBitsShiftXOR((uint)index, seed),
                NoiseType.MangledBitsBalancedMix => MangledBitsBalancedMix((uint)index, seed),
                NoiseType.MangledBitsRotational => MangledBitsRotational((uint)index, seed),
                NoiseType.ChaChaQuarterRoundSimple => ChaChaQuarterRoundSimple((uint)index, seed),
                NoiseType.ChaChaQuarterRoundAdvanced => ChaChaQuarterRoundAdvanced(
                    (uint)index, seed),
                _ => MangledBitsShiftXOR((uint)index, seed)
            };
        }

        /// <summary>
        /// Computes a 2D pseudo-random noise value based on the input coordinates and an optional seed.
        /// This method generates a deterministic random noise value using the given x and y coordinates combined with the seed.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate used as input for generating the noise value.
        /// </param>
        /// <param name="y">
        /// The y-coordinate used as input for generating the noise value.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer used as a seed to influence the randomness of the generated value.
        /// Defaults to 0 if not specified.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A 32-bit unsigned integer representing the computed pseudo-random noise value for the specified 2D coordinates.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get2DNoise(int x, int y, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            Get1DNoise(x + Prime1 * y, seed, type);

        /// <summary>
        /// Generates a deterministic pseudo-random 32-bit unsigned integer value based on the provided 3D coordinates and an optional seed.
        /// This method computes noise by combining the x, y, and z coordinates with prime multipliers, and then applies a 1D noise function.
        /// </summary>
        /// <param name="x">
        /// An integer representing the x-coordinate in 3D space used for the noise calculation.
        /// </param>
        /// <param name="y">
        /// An integer representing the y-coordinate in 3D space used for the noise calculation.
        /// </param>
        /// <param name="z">
        /// An integer representing the z-coordinate in 3D space used for the noise calculation.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer used as a seed to control the randomness of the output. Defaults to 0 if not provided.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A 32-bit unsigned integer representing the deterministic pseudo-random noise value calculated using the input coordinates and seed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get3DNoise(int x, int y, int z, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            Get1DNoise(x + (Prime1 * y) + (Prime2 * z), seed, type);

        /// <summary>
        /// Computes a 32-bit unsigned integer noise value in four dimensions based on the input coordinates and an optional seed.
        /// This function is deterministic and produces consistent results for the same inputs.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate as a signed integer representing the first dimension in the noise space.
        /// </param>
        /// <param name="y">
        /// The y-coordinate as a signed integer representing the second dimension in the noise space.
        /// </param>
        /// <param name="z">
        /// The z-coordinate as a signed integer representing the third dimension in the noise space.
        /// </param>
        /// <param name="w">
        /// The w-coordinate as a signed integer representing the fourth dimension in the noise space.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer used to control the randomness of the noise generation. Defaults to 0.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A 32-bit unsigned integer representing the computed noise value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get4DNoise(int x, int y, int z, int w, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            Get1DNoise(x + (Prime1 * y) + (Prime2 * z) + (Prime3 * w), seed, type);

        #endregion

        #region Float Conversion Helpers

        /// <summary>
        /// Generates a deterministic pseudo-random floating-point value between 0.0 and 1.0, inclusive.
        /// This method is derived from a 1D deterministic noise generation function.
        /// </summary>
        /// <param name="index">
        /// A signed integer representing the input position or index for the noise generation process.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer used as a seed value to alter the randomness deterministically. Defaults to 0 if not provided.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A single-precision floating-point value ranging from 0.0 to 1.0, representing the computed 1D noise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get1DNoise01(int index, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get1DNoise(index, seed, type));

        /// <summary>
        /// Generates a deterministic 2D noise value scaled to the range [0, 1].
        /// This method combines two-dimensional coordinates and an optional seed to produce pseudo-random output.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate for generating the noise value.
        /// </param>
        /// <param name="y">
        /// The y-coordinate for generating the noise value.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer value to alter the randomness. Default is 0.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A floating-point number in the range [0, 1] representing the generated noise value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get2DNoise01(int x, int y, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get2DNoise(x, y, seed, type));

        /// <summary>
        /// Generates a 3D noise value between 0.0 and 1.0 using deterministic pseudo-random algorithms.
        /// This method computes noise based on 3D coordinates and an optional seed value.
        /// </summary>
        /// <param name="x">
        /// An integer representing the X-coordinate in the 3D space for noise generation.
        /// </param>
        /// <param name="y">
        /// An integer representing the Y-coordinate in the 3D space for noise generation.
        /// </param>
        /// <param name="z">
        /// An integer representing the Z-coordinate in the 3D space for noise generation.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer used to alter the randomness of the generated noise.
        /// The default value is 0.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A floating-point value between 0.0 and 1.0 representing the calculated deterministic 3D noise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get3DNoise01(int x, int y, int z, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get3DNoise(x, y, z, seed, type));

        /// <summary>
        /// Computes a 4D noise value normalized to the range [0, 1].
        /// This function generates pseudo-random noise for a four-dimensional space
        /// based on the provided coordinates and an optional seed.
        /// </summary>
        /// <param name="x">
        /// The X-coordinate in the 4D space.
        /// </param>
        /// <param name="y">
        /// The Y-coordinate in the 4D space.
        /// </param>
        /// <param name="z">
        /// The Z-coordinate in the 4D space.
        /// </param>
        /// <param name="w">
        /// The W-coordinate in the 4D space.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer used to control the randomness of the output.
        /// Defaults to 0 if not specified.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A floating-point value between 0 and 1 representing the normalized 4D noise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get4DNoise01(int x, int y, int z, int w, uint seed = 0,
            NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get4DNoise(x, y, z, w, seed, type));

        /// <summary>
        /// Generates a deterministic pseudo-random noise value based on a 1-dimensional index
        /// and an optional seed, normalized to a range between -1 and 1.
        /// </summary>
        /// <param name="index">
        /// A signed integer representing the index or position used to generate the noise value.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer used as a seed for controlling the randomness of the output.
        /// If not specified, the default seed value is 0.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A floating-point value in the range of -1 to 1, representing the normalized pseudo-random noise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get1DNoiseNeg1To1(int index, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get1DNoise(index, seed, type));

        /// <summary>
        /// Generates a deterministic pseudo-random noise value for two-dimensional integer coordinates
        /// and normalizes the result to a floating-point value within the range of -1 to 1.
        /// </summary>
        /// <param name="x">
        /// The X-coordinate used to determine the position for generating noise.
        /// </param>
        /// <param name="y">
        /// The Y-coordinate used to determine the position for generating noise.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer seed to modify the behavior of the noise generation.
        /// The default value is 0.
        /// </param>
        /// <returns>
        /// A floating-point value within the range of -1 to 1, derived from the generated pseudo-random noise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get2DNoiseNeg1To1(int x, int y, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get2DNoise(x, y, seed, type));

        /// <summary>
        /// Generates deterministic pseudo-random noise for a 3D coordinate and normalizes the result within the range of -1 to 1.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate used in the noise generation process.
        /// </param>
        /// <param name="y">
        /// The y-coordinate used in the noise generation process.
        /// </param>
        /// <param name="z">
        /// The z-coordinate used in the noise generation process.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer used to control the randomness and reproducibility of the output. Defaults to 0 if not specified.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A floating-point value within the range of -1 to 1, representing the normalized noise for the given coordinates and seed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get3DNoiseNeg1To1(int x, int y, int z, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get3DNoise(x, y, z, seed, type));

        /// <summary>
        /// Generates a deterministic pseudo-random noise value in 4D space and normalizes it to the range of -1 to 1.
        /// This method ensures consistent results for the same inputs, making it suitable for procedural generation.
        /// </summary>
        /// <param name="x">
        /// The X coordinate, an integer used to determine the noise position in the 4D space.
        /// </param>
        /// <param name="y">
        /// The Y coordinate, an integer used to determine the noise position in the 4D space.
        /// </param>
        /// <param name="z">
        /// The Z coordinate, an integer used to determine the noise position in the 4D space.
        /// </param>
        /// <param name="w">
        /// The W coordinate, an integer used to determine the noise position in the 4D space.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer used as a seed value to control the randomness of the output. Defaults to 0.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A floating-point value in the range of -1 to 1, representing the normalized 4D noise value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get4DNoiseNeg1To1(int x, int y, int z, int w, uint seed = 0,
            NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get4DNoise(x, y, z, w, seed, type));

        #endregion

        #region Perlin

        /// <summary>
        /// Generates a Perlin noise value based on the provided 2D coordinates, seed, and noise type.
        /// The noise value is computed by hashing the corners of the surrounding grid and applying interpolation.
        /// </summary>
        /// <param name="x">
        /// The X coordinate (continuous) used for generating the noise value.
        /// </param>
        /// <param name="y">
        /// The Y coordinate (continuous) used for generating the noise value.
        /// </param>
        /// <param name="seed">
        /// An optional 32-bit unsigned integer seed to enhance the determinism of the noise generation. Defaults to 0 if not provided.
        /// </param>
        /// <param name="type">
        /// The type of noise used to hash the corners of the grid. Defaults to the method's default noise type.
        /// </param>
        /// <returns>
        /// A floating-point value representing the generated Perlin noise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Perlin(float x, float y, uint seed = 0, NoiseType type = DefaultNoiseType)
        {
            var xi = (int)math.floor(x);
            var yi = (int)math.floor(y);
            var xf = x - xi;
            var yf = y - yi;

            // Corner hashes (reuse your integer noise to get stable corner IDs)
            var h00 = Get2DNoise(xi, yi, seed, type);
            var h10 = Get2DNoise(xi + 1, yi, seed, type);
            var h01 = Get2DNoise(xi, yi + 1, seed, type);
            var h11 = Get2DNoise(xi + 1, yi + 1, seed, type);

            return HashBasedNoiseUtils.Perlin(xf, yf, h00, h10, h01, h11);
        }

        /// <summary>
        /// Generates a Perlin noise value based on the input coordinates and a specified seed.
        /// This method calculates smooth noise by blending integer-based deterministic noise
        /// values at each corner of the cube enclosing the input coordinates.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the noise input.
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the noise input.
        /// </param>
        /// <param name="z">
        /// The z-coordinate of the noise input.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer used to control the pseudo-randomness of the noise generation.
        /// </param>
        /// <param name="type">
        /// Specifies the type of noise function to use for generating corner values. Defaults to MangledBitsBalancedMix.
        /// </param>
        /// <returns>
        /// A single-precision floating-point value representing the Perlin noise at the given coordinates.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Perlin(float x, float y, float z, uint seed = 0, NoiseType type = DefaultNoiseType)
        {
            var xi = (int)math.floor(x);
            var yi = (int)math.floor(y);
            var zi = (int)math.floor(z);
            var xf = x - xi;
            var yf = y - yi;
            var zf = z - zi;

            // Corner hashes (reuse your integer noise to get stable corner IDs)
            var h000 = Get3DNoise(xi, yi, zi, seed, type);
            var h100 = Get3DNoise(xi + 1, yi, zi, seed, type);
            var h010 = Get3DNoise(xi, yi + 1, zi, seed, type);
            var h110 = Get3DNoise(xi + 1, yi + 1, zi, seed, type);
            var h001 = Get3DNoise(xi, yi, zi + 1, seed, type);
            var h101 = Get3DNoise(xi + 1, yi, zi + 1, seed, type);
            var h011 = Get3DNoise(xi, yi + 1, zi + 1, seed, type);
            var h111 = Get3DNoise(xi + 1, yi + 1, zi + 1, seed, type);

            return HashBasedNoiseUtils.Perlin(xf, yf, zf, h000, h100, h010, h110, h001, h101, h011, h111);
        }

        #endregion
        
        #region Perlin – Analytic Derivatives

        /// <summary>
        /// Generates a Perlin noise value with derivative, based on the provided 2D coordinates, seed, and noise type.
        /// </summary>
        /// <param name="x">The x-coordinate for the noise calculation.</param>
        /// <param name="y">The y-coordinate for the noise calculation.</param>
        /// <param name="seed">The seed value used for deterministic noise generation.</param>
        /// <param name="d">The derivative of the noise with respect to x and y, returned as a float2.</param>
        /// <param name="type">The type of noise to generate; defaults to the specified <c>DefaultNoiseType</c>.</param>
        /// <returns>A float value representing the generated Perlin noise at the given coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PerlinWithDeriv(float x, float y, uint seed, out float2 d,
            NoiseType type = DefaultNoiseType)
        {
            int xi = (int)math.floor(x), yi = (int)math.floor(y);
            float xf = x - xi, yf = y - yi;

            var h00 = Get2DNoise(xi, yi, seed, type);
            var h10 = Get2DNoise(xi + 1, yi, seed, type);
            var h01 = Get2DNoise(xi, yi + 1, seed, type);
            var h11 = Get2DNoise(xi + 1, yi + 1, seed, type);

            return HashBasedNoiseUtils.PerlinWithDeriv(xf, yf, h00, h10, h01, h11, out d);
        }

        /// <summary>
        /// Computes Perlin noise with derivatives for the given coordinates and seed value.
        /// The method generates smooth noise and calculates gradients based on the noise type.
        /// </summary>
        /// <param name="x">The x-coordinate in the 3D noise space.</param>
        /// <param name="y">The y-coordinate in the 3D noise space.</param>
        /// <param name="z">The z-coordinate in the 3D noise space.</param>
        /// <param name="seed">The seed value used for generating consistent noise output.</param>
        /// <param name="d">The resulting gradients of the Perlin noise, provided as a float3 vector.</param>
        /// <param name="type">The noise type used to control the behavior and mixing of the noise, with a default value.</param>
        /// <returns>The computed Perlin noise value for the provided input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PerlinWithDeriv(float x, float y, float z, uint seed, out float3 d,
            NoiseType type = DefaultNoiseType)
        {
            int xi = (int)math.floor(x), yi = (int)math.floor(y), zi = (int)math.floor(z);
            float xf = x - xi, yf = y - yi, zf = z - zi;

            var h000 = Get3DNoise(xi, yi, zi, seed, type);
            var h100 = Get3DNoise(xi + 1, yi, zi, seed, type);
            var h010 = Get3DNoise(xi, yi + 1, zi, seed, type);
            var h110 = Get3DNoise(xi + 1, yi + 1, zi, seed, type);
            var h001 = Get3DNoise(xi, yi, zi + 1, seed, type);
            var h101 = Get3DNoise(xi + 1, yi, zi + 1, seed, type);
            var h011 = Get3DNoise(xi, yi + 1, zi + 1, seed, type);
            var h111 = Get3DNoise(xi + 1, yi + 1, zi + 1, seed, type);

            return HashBasedNoiseUtils.PerlinWithDeriv(
                xf, yf, zf, h000, h100, h010, h110, h001, h101, h011, h111, out d);
        }
        
        #endregion
        
        #region Finite-Difference Derivatives

        /// <summary>
        /// Computes the value of Perlin noise and its gradient at a given 2D point.
        /// </summary>
        /// <param name="x">The x-coordinate of the input point.</param>
        /// <param name="y">The y-coordinate of the input point.</param>
        /// <param name="seed">A seed value for the noise generation.</param>
        /// <param name="grad">
        /// An output parameter that returns the 2D gradient vector of the noise value at the input point.
        /// </param>
        /// <param name="type">The type of noise to use during computation.</param>
        /// <returns>The Perlin noise value at the given 2D point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ValueAndGrad(float x, float y, uint seed, out float2 grad,
            NoiseType type = DefaultNoiseType)
            => PerlinWithDeriv(x, y, seed, out grad, type);

        /// <summary>
        /// Computes a Perlin noise value at a given 2D position and provides the gradient of the noise at that position.
        /// The method uses a noise function influenced by the provided seed and noise type to generate the result.
        /// </summary>
        /// <param name="p">
        /// A <c>float2</c> representing the 2D position where the noise value is evaluated.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer that determines the seed used for generating the noise.
        /// </param>
        /// <param name="grad">
        /// An output parameter of type <c>float2</c> that contains the gradient of the noise at the given position.
        /// </param>
        /// <param name="type">
        /// The noise generation type that determines the method used for computing the noise values.
        /// Defaults to a predefined type if not provided.
        /// </param>
        /// <returns>
        /// A <c>float</c> representing the computed Perlin noise value at the specified position.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ValueAndGrad(float2 p, uint seed, out float2 grad, NoiseType type = DefaultNoiseType)
            => PerlinWithDeriv(p.x, p.y, seed, out grad, type);

        /// <summary>
        /// Computes the Perlin noise value at the given point and calculates its gradient.
        /// </summary>
        /// <param name="p">The input position represented as a 3D vector.</param>
        /// <param name="seed">The seed value used to ensure deterministic noise generation.</param>
        /// <param name="grad">The resulting gradient of the Perlin noise at the specified position.</param>
        /// <param name="type">The type of noise to be used for computation.</param>
        /// <returns>Returns the Perlin noise value at the given position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ValueAndGrad(float3 p, uint seed, out float3 grad, NoiseType type = DefaultNoiseType)
            => PerlinWithDeriv(p.x, p.y, p.z, seed, out grad, type);

        /// <summary>
        /// Calculates the central gradient of a 2D Perlin noise function at a given point.
        /// This method estimates the gradient by sampling the function in each dimension using
        /// a small step size and computing the finite differences.
        /// </summary>
        /// <param name="p">
        /// A <c>float2</c> instance representing the 2D coordinates where the gradient should be evaluated.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer used as an additional input to control the randomness of the Perlin noise.
        /// </param>
        /// <param name="eps">
        /// A small floating-point value representing the step size used for computing finite differences.
        /// Defaults to <c>1e-3f</c>.
        /// </param>
        /// <param name="type">
        /// The noise generation type that influences the Perlin noise computation.
        /// Optional, defaults to <c>NoiseType.MangledBitsBalancedMix</c>.
        /// </param>
        /// <returns>
        /// A <c>float2</c> instance representing the gradient vector at the specified coordinates.
        /// The x and y components of the returned vector correspond to the gradient along the x and y axes, respectively.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 GradientCentral(float2 p, uint seed, float eps = 1e-3f, NoiseType type = DefaultNoiseType)
        {
            float fx1 = Perlin(p.x + eps, p.y, seed, type);
            float fx0 = Perlin(p.x - eps, p.y, seed, type);
            float fy1 = Perlin(p.x, p.y + eps, seed, type);
            float fy0 = Perlin(p.x, p.y - eps, seed, type);
            return new float2((fx1 - fx0) / (2f * eps), (fy1 - fy0) / (2f * eps));
        }

        /// <summary>
        /// Calculates the central gradient of the Perlin noise function at a given point in 3D space.
        /// It computes the gradient by approximating partial derivatives across all axes using central difference
        /// with a small epsilon step size for numerical differentiation.
        /// </summary>
        /// <param name="p">The 3D point where the gradient is evaluated.</param>
        /// <param name="seed">The seed value used for noise generation, ensuring determinism.</param>
        /// <param name="eps">The small step size used for central difference computation. Default is 1e-3f.</param>
        /// <param name="type">The type of noise function used, defaulting to <see cref="NoiseType.MangledBitsBalancedMix"/>.</param>
        /// <returns>A <see cref="float3"/> vector representing the gradient of the noise function at the specified point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GradientCentral(float3 p, uint seed, float eps = 1e-3f, NoiseType type = DefaultNoiseType)
        {
            var fx1 = Perlin(p.x + eps, p.y, p.z, seed, type);
            var fx0 = Perlin(p.x - eps, p.y, p.z, seed, type);
            var fy1 = Perlin(p.x, p.y + eps, p.z, seed, type);
            var fy0 = Perlin(p.x, p.y - eps, p.z, seed, type);
            var fz1 = Perlin(p.x, p.y, p.z + eps, seed, type);
            var fz0 = Perlin(p.x, p.y, p.z - eps, seed, type);
            return new float3((fx1 - fx0) / (2f * eps), (fy1 - fy0) / (2f * eps), (fz1 - fz0) / (2f * eps));
        }
        
        #endregion
        
        #region Simplex Core (2D / 3D, hash-callback-based)

        /// <summary>
        /// Generates 2D simplex noise at a given position using specified parameters.
        /// The noise function allows adjustable parameters such as seed, frequency, and noise type.
        /// </summary>
        /// <param name="position">The 2D position at which to sample the noise.</param>
        /// <param name="seed">The random seed used for generating the noise. Defaults to 0.</param>
        /// <param name="frequency">The frequency of the noise. Defaults to 1.0.</param>
        /// <param name="type">The type of noise to use, defined by the <see cref="NoiseType"/> enum. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>Returns a single precision float value representing the generated noise at the specified position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Simplex2DCore(float2 position, uint seed = 0, float frequency = 1f,
            NoiseType type = DefaultNoiseType)
        {
            var p = position * frequency;

            // Skew to simplex grid
            var s = (p.x + p.y) * SimplexF2;
            var i = (int)math.floor(p.x + s);
            var j = (int)math.floor(p.y + s);

            // Unskew back to Euclidean
            var t = (i + j) * SimplexG2;
            var x0 = p.x - (i - t);
            var y0 = p.y - (j - t);

            // Determine simplex corner ordering
            var i1 = x0 > y0 ? 1 : 0;
            var j1 = x0 > y0 ? 0 : 1;

            // Offsets for three corners
            var x1 = x0 - i1 + SimplexG2;
            var y1 = y0 - j1 + SimplexG2;
            var x2 = x0 - 1f + 2f * SimplexG2;
            var y2 = y0 - 1f + 2f * SimplexG2;

            // Hash gradients
            var h0 = Get2DNoise(i, j, seed, type);
            var h1 = Get2DNoise(i + i1, j + j1, seed, type);
            var h2 = Get2DNoise(i + 1, j + 1, seed, type);

            float n0 = 0, n1 = 0, n2 = 0;

            var t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 > 0f)
            {
                t0 *= t0;
                var g = Grad2Vec(h0);
                n0 = t0 * t0 * math.dot(g, new float2(x0, y0));
            }

            var t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 > 0f)
            {
                t1 *= t1;
                var g = Grad2Vec(h1);
                n1 = t1 * t1 * math.dot(g, new float2(x1, y1));
            }

            var t2 = 0.5f - x2 * x2 - y2 * y2;
            if (!(t2 > 0f)) return 70f * (n0 + n1 + n2);
            {
                t2 *= t2;
                var g = Grad2Vec(h2);
                n2 = t2 * t2 * math.dot(g, new float2(x2, y2));
            }

            // Scale to roughly [-1,1] like classic Simplex
            return 70f * (n0 + n1 + n2);
        }

        /// <summary>
        /// Generates a 3D simplex noise value based on the input position, seed, frequency, and noise type.
        /// </summary>
        /// <param name="position">The 3D position in space for which the noise value should be calculated.</param>
        /// <param name="seed">A seed value used to initialize the noise generation. Defaults to 0.</param>
        /// <param name="frequency">The frequency of the noise, altering the scale at which it repeats. Defaults to 1.</param>
        /// <param name="type">The specific noise type to be used in the calculation. Uses the default noise type if not specified.</param>
        /// <returns>A floating-point value representing the computed 3D simplex noise at the given position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Simplex3DCore(float3 position, uint seed = 0, float frequency = 1f,
            NoiseType type = DefaultNoiseType)
        {
            var p = position * frequency;

            // Skew to simplex grid
            var s = (p.x + p.y + p.z) * SimplexF3;
            var i = (int)math.floor(p.x + s);
            var j = (int)math.floor(p.y + s);
            var k = (int)math.floor(p.z + s);

            // Unskew
            var t = (i + j + k) * SimplexG3;
            float x0 = p.x - (i - t), y0 = p.y - (j - t), z0 = p.z - (k - t);

            // Determine simplex ordering by ranking x0,y0,z0
            var i1 = x0 >= y0 ? 1 : 0;
            var j1 = x0 >= y0 ? 0 : 1;
            var i2 = x0 >= z0 ? 1 : 0;
            var k1 = x0 >= z0 ? 0 : 1;
            if (y0 >= z0)
            {
                // adjust if y >= z
                if (x0 < y0)
                {
                    i1 = 0;
                    j1 = 1;
                }

                if (x0 < z0)
                {
                    i2 = 0;
                    k1 = 1;
                }
            }
            else
            {
                // y < z
                if (x0 < z0)
                {
                    i2 = 0;
                    k1 = 1;
                }

                if (x0 < y0)
                {
                    i1 = 0;
                    j1 = 1;
                }
            }

            // i1/j1/k1 and i2/j2/k2
            var j2 = (y0 >= z0) ? 1 : 0;
            var k2 = (y0 >= z0) ? 0 : 1;

            // Corner offsets
            var x1 = x0 - i1 + SimplexG3;
            var y1 = y0 - j1 + SimplexG3;
            var z1 = z0 - k1 + SimplexG3;
            var x2 = x0 - i2 + 2f * SimplexG3;
            var y2 = y0 - j2 + 2f * SimplexG3;
            var z2 = z0 - k2 + 2f * SimplexG3;
            var x3 = x0 - 1f + 3f * SimplexG3;
            var y3 = y0 - 1f + 3f * SimplexG3;
            var z3 = z0 - 1f + 3f * SimplexG3;

            // Hash gradients
            var h0 = Get3DNoise(i, j, k, seed, type);
            var h1 = Get3DNoise(i + i1, j + j1, k + k1, seed, type);
            var h2 = Get3DNoise(i + i2, j + j2, k + k2, seed, type);
            var h3 = Get3DNoise(i + 1, j + 1, k + 1, seed, type);

            float n0 = 0, n1 = 0, n2 = 0, n3 = 0;

            var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 > 0f)
            {
                t0 *= t0;
                n0 = t0 * t0 * math.dot(Grad3Vec(h0), new float3(x0, y0, z0));
            }

            var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 > 0f)
            {
                t1 *= t1;
                n1 = t1 * t1 * math.dot(Grad3Vec(h1), new float3(x1, y1, z1));
            }

            var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 > 0f)
            {
                t2 *= t2;
                n2 = t2 * t2 * math.dot(Grad3Vec(h2), new float3(x2, y2, z2));
            }

            var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (!(t3 > 0f)) return 32f * (n0 + n1 + n2 + n3);
            t3 *= t3;
            n3 = t3 * t3 * math.dot(Grad3Vec(h3), new float3(x3, y3, z3));

            return 32f * (n0 + n1 + n2 + n3);
        }
        
        #endregion

        #region Fractals (fBm/Billow/Ridge)

        /// <summary>
        /// Generates a fractal Brownian motion (fBm) value using noise functions and specified parameters.
        /// Combines multiple octaves of noise to produce a fractal-like effect, with adjustable frequency, amplitude, and other properties.
        /// </summary>
        /// <param name="p">The 3D position at which the noise should be evaluated.</param>
        /// <param name="seed">A seed value used for noise generation to ensure repeatability.</param>
        /// <param name="octaves">The number of noise layers (octaves) to combine. Higher values create more detail.</param>
        /// <param name="frequency">The base frequency of the noise function. Higher values increase the noise frequency.</param>
        /// <param name="amplitude">The initial amplitude of the noise. Determines the range of values for the first octave.</param>
        /// <param name="lacunarity">The rate of frequency change between octaves. Defaults to 2, doubling the frequency per octave.</param>
        /// <param name="gain">The rate of amplitude change between octaves. Defaults to 0.5, halving the amplitude per octave.</param>
        /// <param name="normalize">Specifies whether the result should be normalized based on the amplitude sum of all octaves.</param>
        /// <param name="type">The type of noise function to use for generating noise values.</param>
        /// <returns>A floating-point value representing the fBm noise at the given position. Normalized to ~[-1,1] when <paramref name="normalize"/> is true.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FBm(float3 p, uint seed = 0, int octaves = 5, float frequency = 1f,
            float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, bool normalize = true,
            NoiseType type = DefaultNoiseType)
        {
            float sum = 0f, amp = amplitude, freq = frequency;

            for (var i = 0; i < octaves; i++)
            {
                var s = MixOctave(seed, i);
                sum += Perlin(p.x * freq, p.y * freq, p.z * freq, s, type) * amp;
                freq *= lacunarity;
                amp *= gain;
            }

            if (!normalize) return sum;
            var max = AmplitudeSum(octaves, amplitude, gain);
            return (max > 0f) ? sum / max : sum;
        }

        /// <summary>
        /// Generates a 3D Billow noise value by using a series of octaves and applying transformations
        /// to create a smooth, continuous noise pattern. This function allows customization
        /// of frequency, amplitude, lacunarity, gain, and noise type, and can optionally normalize the result.
        /// </summary>
        /// <param name="p">The 3D position input as a float3 vector.</param>
        /// <param name="seed">An optional seed value to initialize noise generation. Default is 0.</param>
        /// <param name="octaves">The number of noise layers or octaves to combine. Default is 5.</param>
        /// <param name="frequency">The base frequency of the noise. Default is 1.</param>
        /// <param name="amplitude">The base amplitude of the noise. Default is 1.</param>
        /// <param name="lacunarity">The frequency multiplier for each successive octave. Default is 2.</param>
        /// <param name="gain">The amplitude multiplier for each successive octave. Default is 0.5.</param>
        /// <param name="normalize">Determines whether to normalize the output to the range [-1, 1]. Default is true.</param>
        /// <param name="type">The type of noise algorithm to use. Default is MangledBitsBalancedMix.</param>
        /// <returns>A float value representing the computed Billow noise, optionally normalized to the range [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Billow(float3 p, uint seed = 0, int octaves = 5, float frequency = 1f,
            float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, bool normalize = true,
            NoiseType type = DefaultNoiseType)
        {
            float sum = 0f, amp = amplitude, freq = frequency;

            for (var i = 0; i < octaves; i++)
            {
                var s = MixOctave(seed, i);
                var n = Perlin(p.x * freq, p.y * freq, p.z * freq, s, type); // [-1,1]
                var b = math.abs(n) * 2f - 1f; // back to [-1,1]
                sum += b * amp;
                freq *= lacunarity;
                amp *= gain;
            }

            if (!normalize) return sum;
            var max = AmplitudeSum(octaves, amplitude, gain);
            return (max > 0f) ? sum / max : sum;
        }

        /// <summary>
        /// Computes a ridge noise value using Perlin noise with specific parameters such as frequency, amplitude,
        /// lacunarity, gain, ridge offset, and ridge sharpness. This function applies multiple octaves of noise
        /// generation and can optionally normalize the output across the range [-1, 1].
        /// </summary>
        /// <param name="p">The input 3D position for the noise function.</param>
        /// <param name="seed">The seed value to ensure determinism in the generation. Default is 0.</param>
        /// <param name="octaves">The number of noise octaves used to compute the ridge noise. Default is 5.</param>
        /// <param name="frequency">The starting frequency for the noise generation. Default is 1f.</param>
        /// <param name="amplitude">The starting amplitude for the noise generation. Default is 1f.</param>
        /// <param name="lacunarity">The frequency multiplier applied after each octave. Default is 2f.</param>
        /// <param name="gain">The amplitude multiplier applied after each octave. Default is 0.5f.</param>
        /// <param name="ridgeOffset">The offset applied to the ridge transformation. Default is 1.0f.</param>
        /// <param name="ridgeSharpness">The sharpness applied to the ridge calculation. Default is 2.0f.</param>
        /// <param name="normalize">
        /// A boolean flag indicating whether the resulting noise values should be normalized
        /// across the range [-1, 1]. Default is true.
        /// </param>
        /// <param name="type">The noise type used for the computation. Default is MangledBitsBalancedMix.</param>
        /// <returns>A float value representing the computed ridge noise. The output can range from [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ridge(float3 p, uint seed = 0, int octaves = 5, float frequency = 1f,
            float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f,
            float ridgeOffset = 1.0f, float ridgeSharpness = 2.0f, bool normalize = true,
            NoiseType type = DefaultNoiseType)
        {
            float sum = 0f, amp = amplitude, freq = frequency;
            var weight = 1f;

            for (var i = 0; i < octaves; i++)
            {
                var s = MixOctave(seed, i);
                var n = Perlin(p.x * freq, p.y * freq, p.z * freq, s, type); // [-1,1]

                var r = ridgeOffset - math.abs(n);
                r = math.max(r, 0f);
                r = math.pow(r, ridgeSharpness);
                r *= weight;

                sum += r * amp;
                weight = math.clamp(r * 0.9f, 0f, 1f);

                freq *= lacunarity;
                amp *= gain;
            }

            if (!normalize)
                return sum; // unnormalized [~0, amplitude sum]

            var max = AmplitudeSum(octaves, amplitude, gain);
            var v = (max > 0f) ? sum / max : sum; // ~[0,1]
            return v * 2f - 1f; // remap to ~[-1,1] for consistency
        }

        /// <summary>
        /// Generates domain-warped noise based on a given position, seed, and various warp and value noise parameters.
        /// Used to create complex, non-repetitive noise patterns by warping the input domain.
        /// </summary>
        /// <param name="p">The input 3D position at which to evaluate the noise.</param>
        /// <param name="seed">The seed value for generating random noise, enabling reproducibility. Defaults to 0.</param>
        /// <param name="warpAmplitude">The amplitude of the warp distortion. Higher values result in more intense warping. Defaults to 1f.</param>
        /// <param name="warpFrequency">The frequency of the warp distortion, controlling the scale of the warp pattern. Defaults to 0.5f.</param>
        /// <param name="warpOctaves">The number of octaves to use for the warp noise. Higher values provide more detail. Defaults to 1.</param>
        /// <param name="warpLacunarity">The lacunarity of the warp noise, affecting the frequency increment between octaves. Defaults to 2f.</param>
        /// <param name="warpGain">The gain of the warp noise, controlling the contribution of each octave. Defaults to 0.5f.</param>
        /// <param name="valueOctaves">The number of octaves to use for the value noise evaluation at the warped position. Defaults to 5.</param>
        /// <param name="valueFrequency">The frequency of the value noise evaluation, controlling the scale of the value pattern. Defaults to 1f.</param>
        /// <param name="valueGain">The gain of the value noise, controlling the contribution of each octave. Defaults to 0.5f.</param>
        /// <param name="valueLacunarity">The lacunarity of the value noise, affecting the frequency increment between octaves. Defaults to 2f.</param>
        /// <param name="type">The type of noise to use for the computations, specifying different blending or mixing mechanisms. Defaults to the default noise type.</param>
        /// <returns>
        /// A single float value representing the computed domain-warped noise at the specified position.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DomainWarp(float3 p, uint seed = 0,
            float warpAmplitude = 1f, float warpFrequency = 0.5f, int warpOctaves = 1,
            float warpLacunarity = 2f, float warpGain = 0.5f,
            int valueOctaves = 5, float valueFrequency = 1f, float valueGain = 0.5f, float valueLacunarity = 2f,
            NoiseType type = DefaultNoiseType)
        {
            // Build a low-freq warp vector (three independent stacks)
            var q = p * warpFrequency;

            var sx = MixOctave(seed, 101);
            var sy = MixOctave(seed, 202);
            var sz = MixOctave(seed, 303);

            var wx = FBm(new float3(q.x + 17f, q.y, q.z),
                sx, warpOctaves, 1f, 1f, warpLacunarity, warpGain, normalize: true, type);
            var wy = FBm(new float3(q.x, q.y + 19f, q.z),
                sy, warpOctaves, 1f, 1f, warpLacunarity, warpGain, normalize: true, type);
            var wz = FBm(new float3(q.x, q.y, q.z + 23f),
                sz, warpOctaves, 1f, 1f, warpLacunarity, warpGain, normalize: true, type);

            var warped = p + warpAmplitude * new float3(wx, wy, wz);

            // Evaluate value stack at the warped position
            return FBm(warped, seed, valueOctaves, valueFrequency, 1f, valueLacunarity, valueGain, normalize: true, type);
        }

        #endregion

        #region Cellular – Per-Cell Seeds (32-bit wrappers)

        /// <summary>
        /// Generates a 2D cell seed using the provided cell coordinates and world seed.
        /// The resulting value is determined by applying a specific noise function.
        /// </summary>
        /// <param name="cell">The 2D coordinates of the cell as an <c>int2</c> structure.</param>
        /// <param name="worldSeed">A seed value that determines the randomization of the noise function. The default value is 0.</param>
        /// <param name="type">The type of noise function to use. The default is <c>NoiseType.MangledBitsBalancedMix</c>.</param>
        /// <returns>A 32-bit unsigned integer representing the cell's seed value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetCellSeed2D(int2 cell, uint worldSeed = 0, NoiseType type = DefaultNoiseType)
            => Get2DNoise(cell.x, cell.y, worldSeed, type);

        /// <summary>
        /// Generates a seed value for a specific 3D cell in space using 3D noise functions.
        /// The resulting value is determined by the cell coordinates, a world seed, and the selected noise type.
        /// </summary>
        /// <param name="cell">The 3D integer coordinates of the cell.</param>
        /// <param name="worldSeed">An optional seed value to differentiate results across different worlds or contexts. Defaults to 0.</param>
        /// <param name="type">The type of noise used for generating the seed. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>Returns a 32-bit unsigned integer representing the generated 3D cell seed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetCellSeed3D(int3 cell, uint worldSeed = 0, NoiseType type = DefaultNoiseType)
            => Get3DNoise(cell.x, cell.y, cell.z, worldSeed, type);

        #endregion

        #region Cellular / Worley

        /// <summary>
        /// Computes 2D cellular noise based on the given input parameters, including position, seed, frequency, jitter, distance metric, and noise type.
        /// The function calculates distances between the input position and feature points within a grid, determining the closest feature points,
        /// their distances, and the corresponding grid cell.
        /// </summary>
        /// <param name="p">The input position in 2D space for computing the cellular noise.</param>
        /// <param name="seed">A seed value for deterministic noise generation. Defaults to 0.</param>
        /// <param name="frequency">The frequency scaling factor for altering the noise detail. Defaults to 1.</param>
        /// <param name="jitter">The amount of displacement applied to feature points within cells. Defaults to 1.</param>
        /// <param name="metric">The metric used to calculate distance between points. Options include Euclidean, Manhattan, and Chebyshev. Defaults to Euclidean.</param>
        /// <param name="type">The noise type dictating the algorithm used for producing hash values. Defaults to the implementation's default noise type.</param>
        /// <returns>A <see cref="HashBasedNoiseUtils.Cellular2DResult"/> representing the computed cellular noise, including distances to feature points, positions, and grid cell data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Cellular2DResult Cellular2D(
            float2 p, uint seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean, NoiseType type = DefaultNoiseType)
        {
            var q = p * frequency;
            var ix = (int)math.floor(q.x);
            var iy = (int)math.floor(q.y);
            var bias = (1f - jitter) * 0.5f;

            float f1 = float.MaxValue, f2 = float.MaxValue;
            int2 bestCell = default;
            float2 bestFeat = default;

            for (var oy = -1; oy <= 1; oy++)
            for (var ox = -1; ox <= 1; ox++)
            {
                var cx = ix + ox;
                var cy = iy + oy;

                var h0 = Get2DNoise(cx, cy, seed, type);
                var h1 = Get2DNoise(cx + Prime1, cy + Prime2, seed, type);

                var r = new float2(ToZeroToOne(h0), ToZeroToOne(h1));
                var feat = new float2(cx, cy) + r * jitter + bias;

                var d = feat - q;
                var dist = metric switch
                {
                    CellularDistance.Manhattan => math.abs(d.x) + math.abs(d.y),
                    CellularDistance.Chebyshev => math.cmax(math.abs(d)),
                    _ => math.length(d)
                };

                if (dist < f1) { f2 = f1; f1 = dist; bestCell = new int2(cx, cy); bestFeat = feat; }
                else if (dist < f2) f2 = dist;
            }

            return new Cellular2DResult(f1, f2, bestFeat, bestCell);
        }

        /// <summary>
        /// Generates a 3D cellular noise pattern based on the given parameters using a specified algorithm.
        /// The result contains the closest two feature points, their distances, coordinates, and origin cells.
        /// </summary>
        /// <param name="p">The position in 3D space where the noise is evaluated.</param>
        /// <param name="seed">An unsigned integer seed value to introduce randomness.</param>
        /// <param name="frequency">The scale of the noise, controlling how detailed the noise pattern appears.</param>
        /// <param name="jitter">A value to adjust the randomness of feature points within each cell; ranges from 0 (no jitter) to 1 (maximum jitter).</param>
        /// <param name="metric">The distance metric used to compute distances between feature points and the input position (e.g., Euclidean, Manhattan, Chebyshev).</param>
        /// <param name="type">The noise generation type dictating the specific algorithm or behavior of the noise function.</param>
        /// <returns>A <c>Cellular3DResult</c> structure containing the closest two feature point distances, the coordinates of the nearest feature point, and the cell indices for the best feature point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Cellular3DResult Cellular3D(
            float3 p, uint seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean, NoiseType type = DefaultNoiseType)
        {
            var q = p * frequency;
            var ix = (int)math.floor(q.x);
            var iy = (int)math.floor(q.y);
            var iz = (int)math.floor(q.z);
            var bias = (1f - jitter) * 0.5f;

            float f1 = float.MaxValue, f2 = float.MaxValue;
            int3 bestCell = default;
            float3 bestFeat = default;

            for (var oz = -1; oz <= 1; oz++)
            for (var oy = -1; oy <= 1; oy++)
            for (var ox = -1; ox <= 1; ox++)
            {
                var cx = ix + ox;
                var cy = iy + oy;
                var cz = iz + oz;

                var h0 = Get3DNoise(cx, cy, cz, seed, type);
                var h1 = Get3DNoise(cx + Prime1, cy + Prime2, cz, seed, type);
                var h2 = Get3DNoise(cx, cy + Prime1, cz + Prime2, seed, type);

                var r = new float3(ToZeroToOne(h0), ToZeroToOne(h1), ToZeroToOne(h2));
                var feat = new float3(cx, cy, cz) + r * jitter + bias;

                var d = feat - q;
                var dist = metric switch
                {
                    CellularDistance.Manhattan => math.abs(d.x) + math.abs(d.y) + math.abs(d.z),
                    CellularDistance.Chebyshev => math.cmax(math.abs(d)),
                    _ => math.length(d)
                };

                if (dist < f1) { f2 = f1; f1 = dist; bestCell = new int3(cx, cy, cz); bestFeat = feat; }
                else if (dist < f2) f2 = dist;
            }

            return new Cellular3DResult(f1, f2, bestFeat, bestCell);
        }

        /// <summary>
        /// Computes the F1 value for cellular noise at a given 2D point.
        /// The F1 value represents the distance to the nearest feature point, offering a basis for cellular noise generation.
        /// Convenience: return F1 only.
        /// </summary>
        /// <param name="p">The 2D point for which the F1 noise value is computed.</param>
        /// <param name="seed">The seed value for generating deterministic noise results.</param>
        /// <param name="frequency">The frequency at which the noise is sampled, affecting the size of the cells.</param>
        /// <param name="jitter">The amount of jitter applied to feature point positions within the cells.</param>
        /// <param name="metric">The distance metric used to compute feature point distances, such as Euclidean or Manhattan.</param>
        /// <param name="type">The type of noise used for hashing and mixing values, e.g., Mangled Bits or ChaCha variants.</param>
        /// <returns>The F1 value, representing the distance to the nearest feature point from the input point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularF1(float2 p, uint seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean, NoiseType type = DefaultNoiseType)
            => Cellular2D(p, seed, frequency, jitter, metric, type).F1;

        /// <summary>
        /// Computes the difference between the second and first closest distances (F2 - F1)
        /// for a 2D cellular noise function at a given point.
        /// Convenience: classic "cell edges": F2 - F1.
        /// </summary>
        /// <param name="p">The 2D point at which the cellular noise function is evaluated.</param>
        /// <param name="seed">The seed value used for the noise calculation. Defaults to 0.</param>
        /// <param name="frequency">The frequency factor applied to the noise function. Defaults to 1f.</param>
        /// <param name="jitter">The amount of jitter applied to cell positions. Defaults to 1f.</param>
        /// <param name="metric">The distance metric used to calculate noise. Defaults to Euclidean.</param>
        /// <param name="type">The noise type used for hashing. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>The difference between the second and first closest distances (F2 - F1) in the cellular noise result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularF2MinusF1(float2 p, uint seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean, NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return r.F2 - r.F1;
        }

        #endregion

        #region Cellular / Worley – High-Level (Wormy / Blocky)

        /// <summary>
        /// Generates a signed 2D Worley noise value based on the given position, seed, frequency, and other parameters.
        /// This function uses cellular edge detection and supports multiple distance metrics for noise computation.
        /// 2D “Wormy” (veiny) Cellular edges in [-1,1].
        /// Defaults: Euclidean, jitter=1.0, freq=0.005, edgeWidth=0.1.
        /// </summary>
        /// <param name="p">The 2D position for which the noise value is computed.</param>
        /// <param name="seed">An optional seed value to introduce variability in noise generation. Defaults to 0.</param>
        /// <param name="frequency">The frequency scale applied to the noise pattern. Defaults to 0.005.</param>
        /// <param name="jitter">The level of jitter applied to the cell points. Higher values increase randomness. Defaults to 1.0.</param>
        /// <param name="metric">Specifies the distance metric used in cellular noise calculations. Defaults to Euclidean.</param>
        /// <param name="edgeWidth">Defines the width of the edges between cells in the noise pattern. Defaults to 0.1.</param>
        /// <param name="type">The type of noise used for computation. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A signed floating-point value representing the computed 2D Worley noise at the specified position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyWormy2D(float2 p, uint seed = 0, float frequency = 0.005f, float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        /// <summary>
        /// Generates a 3D Worley noise value with signed distance based on provided parameters.
        /// This function computes the noise by analyzing the nearest feature points in a 3D space,
        /// applying a jitter and a frequency for distortion, and returns a signed edge value based on
        /// the distance metric and edge width.
        /// 3D “Wormy” (veiny) Cellular edges in [-1,1].
        /// </summary>
        /// <param name="p">The input 3D position for noise computation.</param>
        /// <param name="seed">The seed value for noise generation, used to ensure deterministic results.</param>
        /// <param name="frequency">The frequency scale of the noise pattern, determining its granularity.</param>
        /// <param name="jitter">The amount of perturbation applied to feature points, affecting the noise detail.</param>
        /// <param name="metric">The distance calculation mode (e.g., Euclidean, Manhattan, Chebyshev).</param>
        /// <param name="edgeWidth">The width of the noise's edge transition, influencing edge smoothness.</param>
        /// <param name="type">The noise algorithm used, determining the hashing method and sequence.</param>
        /// <returns>A floating-point value representing the signed edge with applied noise in 3D space.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyWormy3D(float3 p, uint seed = 0, float frequency = 0.006f, float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.12f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        /// <summary>
        /// Generates a 2D Worley noise value with a blocky aesthetic. The noise is evaluated based on the input
        /// position and a variety of parameters influencing the frequency, jitter, distance metric, edge blending,
        /// and noise type.
        /// Defaults: Chebyshev, jitter=0.1, freq=0.01, edgeWidth=0.1.
        /// </summary>
        /// <param name="p">The 2D position at which to evaluate the noise.</param>
        /// <param name="seed">The seed value for randomization. Default is 0.</param>
        /// <param name="frequency">The frequency of the noise pattern. Default is 0.01f.</param>
        /// <param name="jitter">The amount of jitter to apply to cell points. Default is 0.1f.</param>
        /// <param name="metric">The distance metric used to compute cellular distances. Default is Chebyshev.</param>
        /// <param name="edgeWidth">The width of the edge blending between cells. Default is 0.1f.</param>
        /// <param name="type">The type of hash-based noise used for value generation. Default is MangledBitsBalancedMix.</param>
        /// <returns>A float value representing the Worley noise result at the specified position. Cellular edges in [-1,1]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyBlocky2D(float2 p, uint seed = 0, float frequency = 0.01f, float jitter = 0.1f,
            CellularDistance metric = CellularDistance.Chebyshev, float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        /// <summary>
        /// Computes a 3D Worley noise with a blocky appearance using the specified parameters.
        /// The calculation uses Cellular Edge Signed noise with adjustable frequency, jitter, and edge width.
        /// </summary>
        /// <param name="p">The 3D position to sample the noise at.</param>
        /// <param name="seed">An optional seed value to control the randomization of the noise.</param>
        /// <param name="frequency">The frequency scale of the noise.</param>
        /// <param name="jitter">The amount of randomness in the cell positions.</param>
        /// <param name="metric">The cellular distance metric used for computing noise, such as Chebyshev or Euclidean.</param>
        /// <param name="edgeWidth">The width of the edges between cells in the noise.</param>
        /// <param name="type">The noise type used for hashing during calculations.</param>
        /// <returns>A floating-point value representing the computed 3D Worley noise at the given position. Cellular edges in [-1,1]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyBlocky3D(float3 p, uint seed = 0, float frequency = 0.012f, float jitter = 0.1f,
            CellularDistance metric = CellularDistance.Chebyshev, float edgeWidth = 0.12f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        #endregion

        #region Cellular – Normalized F1 Convenience

        /// <summary>
        /// Generates a procedural cellular noise value in the range [0, 1].
        /// The computation is based on a 2D point, with optional parameters controlling
        /// seed value, frequency, jitter, distance metric, and noise type.
        /// Normalized F1 fill in [0,1] (2D)
        /// </summary>
        /// <param name="p">The 2D position for evaluating the cellular noise.</param>
        /// <param name="seed">An optional seed value to control noise generation (default is 0).</param>
        /// <param name="frequency">The frequency to scale the input position by (default is 1).</param>
        /// <param name="jitter">The jitter level to affect cell boundary shapes (default is 1).</param>
        /// <param name="metric">The distance metric used for cellular computation (default is Euclidean).</param>
        /// <param name="type">The noise type determining hashing and mixing behavior (default is MangledBitsBalancedMix).</param>
        /// <returns>A float value in the range [0, 1] representing the computed cellular noise at the input position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularFill01(
            float2 p, uint seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean, NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return NormalizeF101(in r, metric, jitter);
        }

        /// <summary>
        /// Generates a noise-based value mapped to a range of 0 to 1 using cellular noise.
        /// The noise is influenced by parameters for frequency, jitter, distance metric, and noise type.
        /// </summary>
        /// <param name="p">The input 3D point for which noise will be generated.</param>
        /// <param name="seed">A seed value for ensuring deterministic results. Defaults to 0 if not provided.</param>
        /// <param name="frequency">Controls the scale of the noise. Higher values result in smaller features. Defaults to 1.</param>
        /// <param name="jitter">Determines the amount of randomness in cell boundaries. Defaults to 1.</param>
        /// <param name="metric">Specifies the distance metric used in cellular noise computation. Defaults to Euclidean.</param>
        /// <param name="type">Defines the type of noise to apply. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A normalized noise value between 0 and 1 for the given input parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularFill01(
            float3 p, uint seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean, NoiseType type = DefaultNoiseType)
        {
            var r = Cellular3D(p, seed, frequency, jitter, metric, type);
            return NormalizeF101(in r, metric, jitter);
        }

        #endregion

        #region Cellular / Worley – Edge Masks (01 & Signed)

        /// <summary>
        /// Computes a cellular-based noise value mapped to the range [0, 1] with specific edge characteristics.
        /// Allows customization of frequency, jitter, metric, edge width, and noise type.
        /// Great default for “wormy” patterns: Euclidean + jitter=1.0.
        /// </summary>
        /// <param name="p">The input 2D coordinates to evaluate the noise at.</param>
        /// <param name="seed">The seed value for randomization, ensuring deterministic output for the same seed.</param>
        /// <param name="frequency">The frequency of the noise, affecting the scale of features.</param>
        /// <param name="jitter">The jitter factor to introduce randomness into the cellular pattern.</param>
        /// <param name="metric">The distance metric used to compute cell distances (e.g., Euclidean, Manhattan, or Chebyshev).</param>
        /// <param name="edgeWidth">The width of the edge effect in the noise pattern, controlling the transition.</param>
        /// <param name="type">The noise type, specifying the algorithm used for random number generation and value mixing.</param>
        /// <returns>A floating-point value in the range [0, 1], representing the computed cellular noise at the specified input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdge01(float2 p, uint seed = 0, float frequency = 0.005f, float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return CellularVein01(in r, edgeWidth);
        }

        /// <summary>
        /// Calculates a procedural cellular edge effect that transitions smoothly between cell boundaries.
        /// The result is a normalized value in the range [0, 1] where the smoothness of the edge is controlled by the specified edge width.
        /// </summary>
        /// <param name="p">The input 3D position where the cellular noise will be evaluated.</param>
        /// <param name="seed">An optional seed value for controlling the randomness of the cellular noise.</param>
        /// <param name="frequency">The frequency of the cellular noise, which influences the scale of the generated pattern.</param>
        /// <param name="jitter">Controls the variation of cell point positions. A higher value results in more irregular cell distributions.</param>
        /// <param name="metric">The distance metric used for calculating the cellular noise, such as Euclidean or Manhattan.</param>
        /// <param name="edgeWidth">The size of the edge transition, defining how smoothly the noise transitions between cells.</param>
        /// <param name="type">Specifies the noise variation type to be used (e.g., Mangled Bits, Mangled Bits Balanced Mix).</param>
        /// <returns>A normalized float value representing the procedural edge effect at the specified position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdge01(float3 p, uint seed = 0, float frequency = 0.006f, float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.12f,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular3D(p, seed, frequency, jitter, metric, type);
            return CellularVein01(in r, edgeWidth);
        }

        /// <summary>
        /// Computes a value that represents the exponential weighting of cellular edge noise between the first and second feature points,
        /// normalized to the range [0, 1]. The result applies an exponential decay falloff: exp(-k*(F2-F1)) based on the difference between the distances
        /// to the two closest feature points.
        /// Larger k ⇒ thinner/sharper lines.
        /// </summary>
        /// <param name="p">The 2D point in space for which to calculate the noise.</param>
        /// <param name="seed">An optional integer seed used for random generation, which affects the noise calculation.</param>
        /// <param name="frequency">The frequency of the cellular noise. Higher values result in finer noise patterns.</param>
        /// <param name="jitter">The amount of randomness applied to the feature point positions. A value of 1.0 adds full jitter.</param>
        /// <param name="metric">The distance metric used to determine the distance between feature points. Options include Euclidean, Manhattan, and Chebyshev distance.</param>
        /// <param name="k">The exponential scaling factor that determines how quickly the weighting decays based on the feature point distances.</param>
        /// <param name="type">The type of noise algorithm used. Defaults to a balanced mix of mangled bits.</param>
        /// <returns>A floating-point value between 0 and 1 that represents the weighted contribution of the cellular edge noise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeExp01(float2 p, uint seed = 0, float frequency = 0.005f, float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean, float k = 8f, NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return CellularVeinExp01(in r, k);
        }

        /// <summary>
        /// Computes a cell-based edge noise function normalized between 0 and 1, uses exponential weighting
        /// to emphasize cellular vein patterns. The noise is derived based on a 3D coordinate and various parameters
        /// affecting frequency, jitter, and distance metric.
        /// </summary>
        /// <param name="p">A 3D coordinate input.</param>
        /// <param name="seed">A seed value for randomization (default is 0).</param>
        /// <param name="frequency">The frequency of the cellular noise (default is 0.006).</param>
        /// <param name="jitter">The amount of displacement applied to the cell points (default is 0.9).</param>
        /// <param name="metric">The metric used to calculate the distance between cells (default is Euclidean).</param>
        /// <param name="k">An exponential scaling factor influencing the contrast of the noise (default is 8).</param>
        /// <param name="type">The noise type used for hashing (default is MangledBitsBalancedMix).</param>
        /// <returns>A floating-point value normalized between 0 and 1 representing the weighted cell edge noise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeExp01(float3 p, uint seed = 0, float frequency = 0.006f, float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean, float k = 8f, NoiseType type = DefaultNoiseType)
        {
            var r = Cellular3D(p, seed, frequency, jitter, metric, type);
            return CellularVeinExp01(in r, k);
        }

        /// <summary>
        /// Evaluates a signed cellular edge noise value at a given point in 2D space.
        /// The result is derived by scaling and offsetting the unsigned cellular edge noise
        /// to produce a signed range of values.
        /// </summary>
        /// <param name="p">The 2D coordinates at which to evaluate the noise.</param>
        /// <param name="seed">The seed value for generating deterministic noise. Defaults to 0.</param>
        /// <param name="frequency">The frequency of the noise, which determines the scale of noise features. Defaults to 0.005f.</param>
        /// <param name="jitter">The amount of jitter applied to the grid points. Higher values produce more variation. Defaults to 1.0f.</param>
        /// <param name="metric">The distance metric used for cellular noise computations, which affects the shape of features. Defaults to Euclidean.</param>
        /// <param name="edgeWidth">The width of the edge region. This controls the blend between edges and cell interiors. Defaults to 0.1f.</param>
        /// <param name="type">The noise type used for calculations, affecting the mixing and appearance of the final result. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A signed float value representing the cellular edge noise at the given point, normalized to a range of [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeSigned(
            float2 p, uint seed = 0,
            float frequency = 0.005f,
            float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean,
            float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType)
        {
            return CellularEdge01(p, seed, frequency, jitter, metric, edgeWidth, type) * 2f - 1f;
        }

        /// <summary>
        /// Computes a signed cellular noise value by generating patterns of cellular structures.
        /// The output is scaled and shifted to a signed range of -1 to 1.
        /// </summary>
        /// <param name="p">The 3D position input as a float3 vector.</param>
        /// <param name="seed">The seed value used for deterministic randomness. Defaults to 0.</param>
        /// <param name="frequency">The frequency of the noise, controlling the size of individual cells. Defaults to 0.006f.</param>
        /// <param name="jitter">The amount of jitter applied to modify the cell center positions. Defaults to 0.9f.</param>
        /// <param name="metric">The distance metric used to determine cellular patterns (e.g., Euclidean, Manhattan, etc.). Defaults to CellularDistance.Euclidean.</param>
        /// <param name="edgeWidth">The width of the cellular edges. Smaller values produce sharper edges. Defaults to 0.12f.</param>
        /// <param name="type">The noise type used to calculate cellular patterns. Defaults to NoiseType.MangledBitsBalancedMix.</param>
        /// <returns>A float value representing the signed cellular noise, scaled to the range -1 to 1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeSigned(float3 p, uint seed = 0, float frequency = 0.006f, float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.12f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdge01(p, seed, frequency, jitter, metric, edgeWidth, type) * 2f - 1f;

        #endregion

        /// <summary>
        /// Generates a procedural noise value based on chained domain warps and fractional Brownian motion (fBm),
        /// with optional ridge component subtraction. This method produces rich and detailed noise patterns
        /// by combining multiple layers of octaves, domain warping, and ridging, allowing for enhanced control
        /// over frequency, amplitude, and tonal complexity.
        /// "Uber" composition inspired by [![Watch on YouTube](https://img.youtube.com/vi/C9RyEiEzMiU/0.jpg)](https://www.youtube.com/watch?v=C9RyEiEzMiU)
        /// </summary>
        /// <param name="p">The input 3D position for noise evaluation.</param>
        /// <param name="seed">The seed value for noise generation, ensuring repeatability.</param>
        /// <param name="octaves">The number of noise layers (octaves) to compute for fBm.</param>
        /// <param name="frequency">The base frequency of the noise function.</param>
        /// <param name="lacunarity">The frequency multiplier for each successive octave.</param>
        /// <param name="gain">The amplitude multiplier for each successive octave.</param>
        /// <param name="warp1Amp">Amplitude of the first domain warp.</param>
        /// <param name="warp1Freq">Frequency of the first domain warp.</param>
        /// <param name="warp1Oct">Number of octaves used for the first domain warp.</param>
        /// <param name="warp2Amp">Amplitude of the second domain warp.</param>
        /// <param name="warp2Freq">Frequency of the second domain warp.</param>
        /// <param name="warp2Oct">Number of octaves used for the second domain warp.</param>
        /// <param name="ridgeMix">The weight used to mix the ridged noise component with fBm.</param>
        /// <param name="type">The noise type that alters how the procedural noise is calculated.</param>
        /// <returns>A procedural noise value clamped between -1 and 1.</returns>
        public static float UberNoise(float3 p, uint seed = 0, int octaves = 6, float frequency = 1f,
            float lacunarity = 2f, float gain = 0.5f, float warp1Amp = 0.75f, float warp1Freq = 0.5f, int warp1Oct = 2,
            float warp2Amp = 0.35f, float warp2Freq = 1.2f, int warp2Oct = 2, float ridgeMix = 0.3f,
            NoiseType type = DefaultNoiseType)
        {
            // Two chained domain warps (scalar outputs used as uniform offsets)
            var w1 = DomainWarp(p, MixOctave(seed, 101),
                warp1Amp, warp1Freq, warp1Oct, lacunarity, gain,
                valueOctaves: 1, valueFrequency: 1f, valueGain: 0.5f, valueLacunarity: 2f, type);
            var w2 = DomainWarp(p + w1, MixOctave(seed, 202),
                warp2Amp, warp2Freq, warp2Oct, lacunarity, gain,
                valueOctaves: 1, valueFrequency: 1f, valueGain: 0.5f, valueLacunarity: 2f, type);

            var pw = p + w1 + w2;

            // Main fBm minus a ridged component
            var f = FBm(pw, seed, octaves, frequency, 1f, lacunarity, gain, normalize: true, type);
            var r = Ridge(pw * 0.8f, MixOctave(seed, 303),
                math.max(2, octaves / 2), frequency * 1.6f, 1f, lacunarity, gain,
                ridgeOffset: 1.0f, ridgeSharpness: 2.0f, normalize: true, type);

            return math.clamp(f - ridgeMix * r, -1f, 1f);
        }
    }
}