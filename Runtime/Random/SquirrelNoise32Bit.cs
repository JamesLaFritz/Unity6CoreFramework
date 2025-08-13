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

using static CoreFramework.Random.HashBasedNoiseUtils;

namespace CoreFramework.Random
{
    /// <summary>
    /// Provides stateless, deterministic 32-bit noise using integer indexing and optional seeding.
    /// </summary>
    public static class SquirrelNoise32Bit
    {
        #region Constants

        /// <summary>
        /// Defines the default noise type used in the SquirrelNoise algorithm.
        /// This value is utilized when no specific NoiseType is provided to the noise generation methods,
        /// ensuring a consistent and predictable fallback behavior.
        /// </summary>
        private const NoiseType DefaultNoiseType = NoiseType.MangledBitsBalancedMix; // Default noise type to use
        
        #endregion

        // ToDo: Add ulong4(Get256)
        // ToDo: possible ulong8(GetUint512), and ulong16(GetUint1024)

        /// <summary>
        /// Computes a 128-bit unsigned integer by combining two deterministic pseudo-random 64-bit unsigned integers.
        /// This method generates low and high components based on the input index and seed through multi-dimensional noise computation.
        /// </summary>
        /// <param name="index">
        /// A signed integer used as the position or index to generate pseudo-random noise.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer used as an additional value to control the randomness of the output.
        /// </param>
        /// <param name="type">
        /// The noise generation type, which defines the noise computation method and mixing function.
        /// </param>
        /// <returns>
        /// A 128-bit unsigned integer represented as a struct containing two 64-bit pseudo-random noise components.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 GetUInt128(int index, uint seed, NoiseType type = DefaultNoiseType)
        {
            //Unity.Mathematics.uint4 temp = new uint4(uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue);
            var lo = GetUInt64(index, seed, type);
            return new ulong2(GetUInt64(index + (int)(Prime1 * lo), seed, type), lo);
        }

        /// <summary>
        /// Computes a 64-bit unsigned integer by combining two deterministic pseudo-random 32-bit unsigned integers.
        /// This method generates low and high components based on the input index and seed.
        /// </summary>
        /// <param name="index">
        /// A signed integer used as the position or index to generate pseudo-random noise.
        /// </param>
        /// <param name="seed">
        /// A 32-bit unsigned integer used as an additional value to control the randomness of the output.
        /// </param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>
        /// A 64-bit unsigned integer representing the combined pseudo-random noise value.
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
                NoiseType.MangledBits => HashBasedNoiseUtils.MangledBitsShiftXOR((uint)index, seed),
                NoiseType.MangledBitsBalancedMix => HashBasedNoiseUtils.MangledBitsBalancedMix((uint)index, seed),
                NoiseType.MangledBitsRotational => HashBasedNoiseUtils.MangledBitsRotational((uint)index, seed),
                NoiseType.ChaChaQuarterRoundSimple => HashBasedNoiseUtils.ChaChaQuarterRoundSimple((uint)index, seed),
                NoiseType.ChaChaQuarterRoundAdvanced => HashBasedNoiseUtils.ChaChaQuarterRoundAdvanced(
                    (uint)index, seed),
                _ => HashBasedNoiseUtils.MangledBitsShiftXOR((uint)index, seed)
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
            Get1DNoise(x + (Prime1 * y), seed);

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
            Get1DNoise(x + (Prime1 * y) + (Prime2 * z), seed);

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
            Get1DNoise(x + (Prime1 * y) + (Prime2 * z) + (Prime3 * w), seed);

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
            HashBasedNoiseUtils.ToZeroToOne(Get1DNoise(index, seed));

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
            HashBasedNoiseUtils.ToZeroToOne(Get2DNoise(x, y, seed));

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
            HashBasedNoiseUtils.ToZeroToOne(Get3DNoise(x, y, z, seed));

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
        public static float Get4DNoise01(int x, int y, int z, int w, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            HashBasedNoiseUtils.ToZeroToOne(Get4DNoise(x, y, z, w, seed));

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
            HashBasedNoiseUtils.ToNegOneToOne(Get1DNoise(index, seed));

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
            HashBasedNoiseUtils.ToNegOneToOne(Get2DNoise(x, y, seed));

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
            HashBasedNoiseUtils.ToNegOneToOne(Get3DNoise(x, y, z, seed));

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
        public static float Get4DNoiseNeg1To1(int x, int y, int z, int w, uint seed = 0, NoiseType type = DefaultNoiseType) =>
            HashBasedNoiseUtils.ToNegOneToOne(Get4DNoise(x, y, z, w, seed));

        #endregion
    }
}