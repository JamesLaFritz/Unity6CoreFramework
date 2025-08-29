#region Header
// SecureNoise64.cs
// Author:
//         Original concept based on Squirrel Eiserloh's noise-based RNG
//         64-bit extended version by James LaFritz
// Description: A 64-bit noise-based pseudo-random number generator for deterministic applications.
// Designed for stateless, parallel-safe, reproducible random access noise.
// Matches structure of SquirrelNoise.cs.
#endregion

using System.Runtime.CompilerServices;
using CoreFramework.Mathematics;
using Unity.Burst;
using Unity.Mathematics;
using static CoreFramework.Random.HashBasedNoiseUtils;

namespace CoreFramework.Random
{
    /// <summary>
    /// Provides high-entropy stateless deterministic noise using a ChaCha-inspired mixing function.
    /// </summary>
    [BurstCompile]
    public static class SquirrelNoise64Bit
    {
        #region Constants

        /// <summary>
        /// A constant that defines the default noise type used in the SquirrelNoise64Bit methods.
        /// The default value is set to <see cref="NoiseType.MangledBitsBalancedMix"/>.
        /// </summary>
        private const NoiseType DefaultNoiseType = NoiseType.MangledBitsBalancedMix; // Default noise type to use

        #endregion

        // ToDo: Add ulong2(GetUint128) and ulong4(Get256)
        // ToDo: possible ulong8(GetUint512), and ulong16(GetUint1024)

        /// <summary>
        /// Computes a 128-bit unsigned integer noise value consisting of two 64-bit parts, based on the provided index, seed, and noise generation type.
        /// </summary>
        /// <param name="index">The index value used to compute the noise, influencing the position or state of the generated noise.</param>
        /// <param name="seed">A 64-bit unsigned integer that adds randomness to the computed noise.</param>
        /// <param name="type">The noise generation type, which determines the method and mixing algorithm used for noise computation.</param>
        /// <returns>A 128-bit unsigned integer represented as a struct containing two 64-bit parts of computed noise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 GetUInt128(ulong index, ulong seed, NoiseType type = DefaultNoiseType)
        {
            var lo = GetUInt64(index, seed, type);
            return new ulong2(GetUInt64(index + Prime1Ul * lo, seed, type), lo);
        }

        /// <summary>
        /// Computes a 64-bit unsigned integer noise value based on the provided index, seed, and noise generation type.
        /// </summary>
        /// <param name="index">The index value used to compute the noise, influencing the position or state of the generated noise.</param>
        /// <param name="seed">A 64-bit unsigned integer that adds randomness to the computed noise.</param>
        /// <param name="type">The noise generation type, which determines the method and algorithm used for noise computation. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A 64-bit unsigned integer representing the computed noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetUInt64(ulong index, ulong seed, NoiseType type = DefaultNoiseType)
        {
            return type switch
            {
                NoiseType.MangledBits => MangledBitsShiftXOR(index, seed),
                NoiseType.MangledBitsBalancedMix => MangledBitsBalancedMix(index, seed),
                NoiseType.MangledBitsRotational => MangledBitsRotational(index, seed),
                NoiseType.ChaChaQuarterRoundSimple => ChaChaQuarterRoundSimple(index, seed),
                NoiseType.ChaChaQuarterRoundAdvanced => ChaChaQuarterAdvanced(
                    index, seed),
                _ => MangledBitsShiftXOR(index, seed)
            };
        }

        /// <summary>
        /// Computes a 1D noise value as a 32-bit unsigned integer based on the given index, seed, and noise generation algorithm type.
        /// </summary>
        /// <param name="index">The input index used to determine the specific noise value computation.</param>
        /// <param name="seed">A 64-bit unsigned integer used as a seed for generating deterministic noise.</param>
        /// <param name="type">The noise generation type, specified by the <see cref="NoiseType"/> enumeration, which determines the algorithm and mixing strategy employed during computation.</param>
        /// <returns>A 32-bit unsigned integer representing the computed 1D noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get1DNoise(ulong index, ulong seed, NoiseType type = DefaultNoiseType)
        {
            var noise64 = GetUInt64(index, seed, type);

            var lo32 = (uint)noise64;
            var hi32 = (uint)(noise64 >> 32);
            return lo32 ^ (hi32 * GoldenRatio);
        }

        #region Higher-Dimensional Noise

        /// <summary>
        /// Generates a 2D noise value based on the given x and y coordinates and an optional seed using a deterministic algorithm.
        /// </summary>
        /// <param name="x">The x-coordinate of the point for which to generate 2D noise.</param>
        /// <param name="y">The y-coordinate of the point for which to generate 2D noise.</param>
        /// <param name="seed">An optional seed value to modify the noise output. Defaults to 0.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 32-bit unsigned integer representing the generated 2D noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get2DNoise(ulong x, ulong y, ulong seed = 0, NoiseType type = DefaultNoiseType) =>
            Get1DNoise(x + Prime1Ul * y, seed, type);

        /// <summary>
        /// Generates a pseudo-random 3D noise value based on the specified coordinates and seed.
        /// </summary>
        /// <param name="x">The X-coordinate for the 3D noise.</param>
        /// <param name="y">The Y-coordinate for the 3D noise.</param>
        /// <param name="z">The Z-coordinate for the 3D noise.</param>
        /// <param name="seed">An optional seed value to influence the noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A pseudo-random 3D noise value as a 32-bit unsigned integer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get3DNoise(ulong x, ulong y, ulong z, ulong seed = 0, NoiseType type = DefaultNoiseType) =>
            Get1DNoise(x + Prime1Ul * y + Prime2Ul * z, seed, type);

        /// <summary>
        /// Generates a pseudo-random 4D noise value based on the specified coordinates and seed.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-coordinate.</param>
        /// <param name="w">The w-coordinate.</param>
        /// <param name="seed">The seed used to initialize the noise generation. Default is 0.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 32-bit unsigned integer representing the generated 4D noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get4DNoise(ulong x, ulong y, ulong z, ulong w, ulong seed = 0,
            NoiseType type = DefaultNoiseType) =>
            Get1DNoise(x + Prime1Ul * y + Prime2Ul * z + Prime3Ul * w, seed, type);

        #endregion

        #region Float Conversion Helpers

        /// <summary>
        /// Generates a 1D noise value normalized to the range [0, 1].
        /// </summary>
        /// <param name="index">The index used to compute the noise value.</param>
        /// <param name="seed">The seed value for randomization, defaulting to 0 if not provided.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A single-precision floating-point noise value in the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get1DNoise01(ulong index, ulong seed = 0, NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get1DNoise(index, seed, type));

        /// <summary>
        /// Generates a 2D procedural noise value normalized to the range [0, 1].
        /// </summary>
        /// <param name="x">The x-coordinate of the input position.</param>
        /// <param name="y">The y-coordinate of the input position.</param>
        /// <param name="seed">A value used to seed the noise generation process, defaulting to 0.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A float value representing the 2D noise, normalized to the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get2DNoise01(ulong x, ulong y, ulong seed = 0, NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get2DNoise(x, y, seed, type));

        /// <summary>
        /// Generates a 3D noise value mapped to the range [0, 1].
        /// </summary>
        /// <param name="x">The x-coordinate of the 3D point.</param>
        /// <param name="y">The y-coordinate of the 3D point.</param>
        /// <param name="z">The z-coordinate of the 3D point.</param>
        /// <param name="seed">An optional seed value for noise generation. Default is 0.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A pseudo-random noise value in the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get3DNoise01(ulong x, ulong y, ulong z, ulong seed = 0,
            NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get3DNoise(x, y, z, seed, type));

        /// <summary>
        /// Generates 4D noise as a float value in the range [0, 1].
        /// </summary>
        /// <param name="x">The x-coordinate of the 4D noise.</param>
        /// <param name="y">The y-coordinate of the 4D noise.</param>
        /// <param name="z">The z-coordinate of the 4D noise.</param>
        /// <param name="w">The w-coordinate of the 4D noise.</param>
        /// <param name="seed">The seed value used for randomizing the noise generation. Defaults to 0 if not specified.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A float value representing the 4D noise in the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get4DNoise01(ulong x, ulong y, ulong z, ulong w, ulong seed = 0,
            NoiseType type = DefaultNoiseType) =>
            ToZeroToOne(Get4DNoise(x, y, z, w, seed, type));

        /// <summary>
        /// Generates a 1D noise value normalized to the range of -1 to 1.
        /// </summary>
        /// <param name="index">The index used to calculate the noise value.</param>
        /// <param name="seed">The seed value for ensuring deterministic noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point noise value in the range of -1 to 1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get1DNoiseNeg1To1(ulong index, ulong seed = 0, NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get1DNoise(index, seed, type));

        /// <summary>
        /// Generates a 2D noise value within the range [-1, 1] based on the provided coordinates and seed.
        /// </summary>
        /// <param name="x">The x-coordinate used for the noise generation.</param>
        /// <param name="y">The y-coordinate used for the noise generation.</param>
        /// <param name="seed">An optional seed value for the noise function. Defaults to 0.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point noise value in the range [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get2DNoiseNeg1To1(ulong x, ulong y, ulong seed = 0, NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get2DNoise(x, y, seed, type));

        /// <summary>
        /// Generates 3D noise in the range of -1 to 1 based on x, y, z coordinates and a seed value.
        /// </summary>
        /// <param name="x">The x-coordinate of the noise point.</param>
        /// <param name="y">The y-coordinate of the noise point.</param>
        /// <param name="z">The z-coordinate of the noise point.</param>
        /// <param name="seed">An optional seed value to initialize the noise generation, defaulting to 0.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point noise value in the range of -1 to 1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get3DNoiseNeg1To1(ulong x, ulong y, ulong z, ulong seed = 0,
            NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get3DNoise(x, y, z, seed, type));

        /// <summary>
        /// Generates a 4D noise value mapped to the range [-1, 1].
        /// </summary>
        /// <param name="x">The x-coordinate in the 4D space.</param>
        /// <param name="y">The y-coordinate in the 4D space.</param>
        /// <param name="z">The z-coordinate in the 4D space.</param>
        /// <param name="w">The w-coordinate in the 4D space.</param>
        /// <param name="seed">The seed value to control the noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point noise value in the range [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get4DNoiseNeg1To1(ulong x, ulong y, ulong z, ulong w, ulong seed = 0,
            NoiseType type = DefaultNoiseType) =>
            ToNegOneToOne(Get4DNoise(x, y, z, w, seed, type));

        #endregion

        #region Perlin

        /// <summary>
        /// Computes Perlin noise value for the specified 2D coordinates (x, y) using the given seed and noise type.
        /// </summary>
        /// <param name="x">The x-coordinate of the sample point in the 2D noise field.</param>
        /// <param name="y">The y-coordinate of the sample point in the 2D noise field.</param>
        /// <param name="seed">An optional 64-bit unsigned integer seed providing additional randomness to the noise output. Defaults to 0.</param>
        /// <param name="type">The noise generation algorithm type used for computing the noise. Defaults to the system's default noise type.</param>
        /// <returns>A single-precision floating-point value representing the computed Perlin noise at the specified coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Perlin(float x, float y, ulong seed = 0, NoiseType type = DefaultNoiseType)
        {
            // Use double for inputs if you prefer; cast to float for core math
            var xi = (ulong)math.floor(x);
            var yi = (ulong)math.floor(y);
            var xf = x - (float)xi;
            var yf = y - (float)yi;

            // Corner hashes (reuse your integer noise to get stable corner IDs)
            var h00 = Get2DNoise(xi, yi, seed, type);
            var h10 = Get2DNoise(xi + 1UL, yi, seed, type);
            var h01 = Get2DNoise(xi, yi + 1UL, seed, type);
            var h11 = Get2DNoise(xi + 1UL, yi + 1UL, seed, type);

            return HashBasedNoiseUtils.Perlin(xf, yf, h00, h10, h01, h11);
        }

        /// <summary>
        /// Computes a Perlin noise value for a specific 3D point using optional settings for seed value and noise computation type.
        /// </summary>
        /// <param name="x">The X-coordinate of the input point in 3D space.</param>
        /// <param name="y">The Y-coordinate of the input point in 3D space.</param>
        /// <param name="z">The Z-coordinate of the input point in 3D space.</param>
        /// <param name="seed">An optional 64-bit unsigned seed value for adding randomness, defaulting to 0.</param>
        /// <param name="type">An optional noise type specifying the computation method, defaulting to a balanced mix.</param>
        /// <returns>A single-precision floating-point value representing the computed Perlin noise at the given 3D coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Perlin(float x, float y, float z, ulong seed = 0, NoiseType type = DefaultNoiseType)
        {
            var xi = (ulong)math.floor(x);
            var yi = (ulong)math.floor(y);
            var zi = (ulong)math.floor(z);
            var xf = x - (float)xi;
            var yf = y - (float)yi;
            var zf = z - (float)zi;

            // Corner hashes (reuse your integer noise to get stable corner IDs)
            var h000 = Get3DNoise(xi, yi, zi, seed, type);
            var h100 = Get3DNoise(xi + 1UL, yi, zi, seed, type);
            var h010 = Get3DNoise(xi, yi + 1UL, zi, seed, type);
            var h110 = Get3DNoise(xi + 1UL, yi + 1UL, zi, seed, type);
            var h001 = Get3DNoise(xi, yi, zi + 1UL, seed, type);
            var h101 = Get3DNoise(xi + 1UL, yi, zi + 1UL, seed, type);
            var h011 = Get3DNoise(xi, yi + 1UL, zi + 1UL, seed, type);
            var h111 = Get3DNoise(xi + 1UL, yi + 1UL, zi + 1UL, seed, type);

            return HashBasedNoiseUtils.Perlin(xf, yf, zf, h000, h100, h010, h110, h001, h101, h011, h111);
        }

        #endregion
        
        #region Perlin – Analytic Derivatives

        /// <summary>
        /// Computes a Perlin noise value at the specified 2D coordinates with derivatives, based on the provided seed and noise type.
        /// </summary>
        /// <param name="x">The X-coordinate of the input position.</param>
        /// <param name="y">The Y-coordinate of the input position.</param>
        /// <param name="seed">A 64-bit unsigned integer used as a randomization seed for the noise generation.</param>
        /// <param name="d">An output parameter that contains the 2D derivatives of the noise in the X and Y directions.</param>
        /// <param name="type">The noise generation type, which defines the method used for noise computation and mixing.</param>
        /// <returns>A floating-point value representing the computed Perlin noise at the specified coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PerlinWithDeriv(float x, float y, ulong seed, out float2 d,
            NoiseType type = DefaultNoiseType)
        {
            ulong xi = (ulong)math.floor(x), yi = (ulong)math.floor(y);
            float xf = x - (float)xi, yf = y - (float)yi;

            var h00 = Get2DNoise(xi, yi, seed, type);
            var h10 = Get2DNoise(xi + 1, yi, seed, type);
            var h01 = Get2DNoise(xi, yi + 1, seed, type);
            var h11 = Get2DNoise(xi + 1, yi + 1, seed, type);

            return HashBasedNoiseUtils.PerlinWithDeriv(xf, yf, h00, h10, h01, h11, out d);
        }

        /// <summary>
        /// Computes Perlin noise with derivatives for the given 3D position, seed, and noise generation type.
        /// </summary>
        /// <param name="x">The x-coordinate of the position in 3D space for noise computation.</param>
        /// <param name="y">The y-coordinate of the position in 3D space for noise computation.</param>
        /// <param name="z">The z-coordinate of the position in 3D space for noise computation.</param>
        /// <param name="seed">A 64-bit unsigned integer seed value to control the randomness of the noise behavior.</param>
        /// <param name="d">An output parameter representing the 3D gradient of the noise at the specified position.</param>
        /// <param name="type">The noise generation type determining how the noise and gradients are computed.</param>
        /// <returns>A single-precision floating-point value representing the computed Perlin noise at the specified position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PerlinWithDeriv(float x, float y, float z, ulong seed, out float3 d,
            NoiseType type = DefaultNoiseType)
        {
            ulong xi = (ulong)math.floor(x), yi = (ulong)math.floor(y), zi = (ulong)math.floor(z);
            float xf = x - (float)xi, yf = y - (float)yi, zf = z - (float)zi;

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
        public static float ValueAndGrad(float x, float y, ulong seed, out float2 grad,
            NoiseType type = DefaultNoiseType)
            => PerlinWithDeriv(x, y, seed, out grad, type);

        /// <summary>
        /// Computes the Perlin noise value at a given 2D coordinate, along with its gradient at that point, based on the provided seed and noise generation type.
        /// </summary>
        /// <param name="p">A 2D coordinate represented as a float2 structure, specifying the point for which the noise value and gradient are computed.</param>
        /// <param name="seed">A 64-bit unsigned integer that introduces randomness into the noise generation process.</param>
        /// <param name="grad">An output parameter that stores the computed gradient as a float2 structure for the given 2D point.</param>
        /// <param name="type">The noise generation type, determining the algorithm and mixing method used for computing the noise value and gradient.</param>
        /// <returns>A single-precision floating-point value representing the computed Perlin noise at the specified 2D coordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ValueAndGrad(float2 p, ulong seed, out float2 grad, NoiseType type = DefaultNoiseType)
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
        public static float ValueAndGrad(float3 p, ulong seed, out float3 grad, NoiseType type = DefaultNoiseType)
            => PerlinWithDeriv(p.x, p.y, p.z, seed, out grad, type);

        /// <summary>
        /// Computes the central gradient of a 2D Perlin noise function at a given position, using small directional offsets along the x and y axes.
        /// </summary>
        /// <param name="p">The 2D position at which the gradient should be calculated.</param>
        /// <param name="seed">A 64-bit unsigned integer value that provides randomness for the Perlin noise calculation.</param>
        /// <param name="eps">A small offset value used for finite difference calculations to approximate the derivatives. Default is 1e-3.</param>
        /// <param name="type">The noise generation type, determining the algorithm used for noise computation. Default is MangledBitsBalancedMix.</param>
        /// <returns>A float2 structure representing the gradient vector at the specified position, with x and y components calculated based on Perlin noise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 GradientCentral(float2 p, ulong seed, float eps = 1e-3f, NoiseType type = DefaultNoiseType)
        {
            var fx1 = Perlin(p.x + eps, p.y, seed, type);
            var fx0 = Perlin(p.x - eps, p.y, seed, type);
            var fy1 = Perlin(p.x, p.y + eps, seed, type);
            var fy0 = Perlin(p.x, p.y - eps, seed, type);
            return new float2((fx1 - fx0) / (2f * eps), (fy1 - fy0) / (2f * eps));
        }

        /// <summary>
        /// Calculates the central gradient of the Perlin noise function at a given point in 3D space.
        /// It computes the gradient by approximating partial derivatives across all axes using a central difference
        /// with a small epsilon step size for numerical differentiation.
        /// </summary>
        /// <param name="p">The 3D point where the gradient is evaluated.</param>
        /// <param name="seed">The seed value used for noise generation, ensuring determinism.</param>
        /// <param name="eps">The small step size used for central difference computation. The default is 1e-3f.</param>
        /// <param name="type">The type of noise function used, defaulting to <see cref="NoiseType.MangledBitsBalancedMix"/>.</param>
        /// <returns>A <see cref="float3"/> vector representing the gradient of the noise function at the specified point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GradientCentral(float3 p, ulong seed, float eps = 1e-3f, NoiseType type = DefaultNoiseType)
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
        /// Computes a 2D Simplex noise value based on the specified position, seed, frequency, and noise type.
        /// </summary>
        /// <param name="position">The 2D coordinates at which the noise value is to be computed.</param>
        /// <param name="seed">A 64-bit unsigned integer used to initialize the random sequence for noise generation. Defaults to 0.</param>
        /// <param name="frequency">A scaling factor that determines the spatial frequency of the noise. Defaults to 1.</param>
        /// <param name="type">The noise computation type, influencing the gradient mixing method. Defaults to <see cref="DefaultNoiseType"/>.</param>
        /// <returns>A floating-point number representing the computed Simplex noise value, scaled to approximately the range [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Simplex2DCore(float2 position, ulong seed = 0, float frequency = 1f,
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
            var h0 = Get2DNoise((ulong)i, (ulong)j, seed, type);
            var h1 = Get2DNoise((ulong)(i + i1), (ulong)(j + j1), seed, type);
            var h2 = Get2DNoise((ulong)(i + 1), (ulong)(j + 1), seed, type);

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
        /// Calculates a 3D Simplex noise value at the given position using the specified seed, frequency, and noise type.
        /// </summary>
        /// <param name="position">The 3D position at which to evaluate the noise function.</param>
        /// <param name="seed">An optional 64-bit unsigned integer to seed the noise generation, providing deterministic randomness.</param>
        /// <param name="frequency">The frequency of the noise, scaling the input position to control the level of detail.</param>
        /// <param name="type">The noise generation type, determining the method used for gradient computation and noise variation.</param>
        /// <returns>A single-precision floating-point value representing the computed 3D Simplex noise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Simplex3DCore(float3 position, ulong seed = 0, float frequency = 1f,
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
            var h0 = Get3DNoise((ulong)i, (ulong)j, (ulong)k, seed, type);
            var h1 = Get3DNoise((ulong)(i + i1), (ulong)(j + j1), (ulong)(k + k1), seed, type);
            var h2 = Get3DNoise((ulong)(i + i2), (ulong)(j + j2), (ulong)(k + k2), seed, type);
            var h3 = Get3DNoise((ulong)(i + 1), (ulong)(j + 1), (ulong)(k + 1), seed, type);

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
        public static float FBm(float3 p, ulong seed = 0, int octaves = 5, float frequency = 1f, float amplitude = 1f,
            float lacunarity = 2f, float gain = 0.5f, bool normalize = true, NoiseType type = DefaultNoiseType)
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
        public static float Billow(float3 p, ulong seed = 0, int octaves = 5,
            float frequency = 1f, float amplitude = 1f, float lacunarity = 2f,
            float gain = 0.5f, bool normalize = true, NoiseType type = DefaultNoiseType)
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
        public static float Ridge(float3 p, ulong seed = 0, int octaves = 5,
            float frequency = 1f, float amplitude = 1f, float lacunarity = 2f,
            float gain = 0.5f, float ridgeOffset = 1f, float ridgeSharpness = 2f,
            bool normalize = true, NoiseType type = DefaultNoiseType)
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
        /// Applies domain warping to a position in 3D space using fractal Brownian motion (FBm) as the primary technique,
        /// resulting in a distorted yet consistent noise value.
        /// </summary>
        /// <param name="p">The input 3D position to be warped, represented as a float3 structure.</param>
        /// <param name="seed">An optional 64-bit unsigned integer seed used to randomize the noise generation process. Default is 0.</param>
        /// <param name="warpAmplitude">The amplitude of the warp, which controls the intensity of the distortion. Default is 1f.</param>
        /// <param name="warpFrequency">The frequency of the domain warp, influencing the granularity of the warp. Default is 0.5f.</param>
        /// <param name="warpOctaves">The number of warp octaves, which controls the layering of warp detail. Default is 1.</param>
        /// <param name="warpLacunarity">The lacunarity scaling factor between warp octaves. Higher values increase the frequency of each sequential octave. Default is 2f.</param>
        /// <param name="warpGain">The gain factor that controls the contribution of each subsequent warp octave to the overall warp effect. Default is 0.5f.</param>
        /// <param name="valueOctaves">The number of octaves used for the evaluation of the warped position, which affects noise detail. Default is 5.</param>
        /// <param name="valueFrequency">The frequency used in evaluating the final noise value at the warped position. Default is 1f.</param>
        /// <param name="valueGain">The gain factor for the evaluation of noise value octaves. It influences the amplitude of subsequent octaves. Default is 0.5f.</param>
        /// <param name="valueLacunarity">The lacunarity scaling factor for the evaluation of noise value octaves. Default is 2f.</param>
        /// <param name="type">The noise generation type, which determines the algorithm used for noise computation. Default is MangledBitsBalancedMix.</param>
        /// <returns>A single-precision floating-point value representing the computed noise at the warped 3D position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DomainWarp(float3 p, ulong seed = 0,
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

        #region Cellular – Per-Cell Seeds (64-bit wrappers)

        /// <summary>
        /// Generates a 64-bit hash-based seed for a specified 2D cell coordinate using a given world seed and noise generation type.
        /// </summary>
        /// <param name="cell">The 2D cell coordinate specified as an <see cref="int2"/> structure, which determines the location for seed generation.</param>
        /// <param name="worldSeed">An optional 64-bit unsigned integer used to introduce additional randomness into the generated seed. Defaults to 0 if not specified.</param>
        /// <param name="type">Specifies the noise generation algorithm used to compute the seed, defaulting to <see cref="NoiseType.MangledBitsBalancedMix"/>.</param>
        /// <returns>Returns a 64-bit unsigned integer representing the computed seed value for the specified 2D cell.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetCellSeed2D(int2 cell, ulong worldSeed = 0, NoiseType type = DefaultNoiseType)
        {
            // Compose an index in 64-bit space and hash via GetUInt64 for a true 64-bit seed.
            var idx = (ulong)cell.x + Prime1Ul * (ulong)cell.y;
            return GetUInt64(idx, worldSeed, type);
        }

        /// <summary>
        /// Computes a 64-bit noise value specific to a 3D cell location, based on the provided cell coordinates, world seed, and noise generation type.
        /// </summary>
        /// <param name="cell">The 3D cell coordinates represented as an int3 structure, specifying the x, y, and z positions.</param>
        /// <param name="worldSeed">A 64-bit unsigned integer seed applied to add randomness to the noise generation. Default is 0.</param>
        /// <param name="type">The noise generation type that determines the algorithm and mixing used for computation. Default is MangledBitsBalancedMix.</param>
        /// <returns>A 64-bit unsigned integer representing the computed noise for the specified 3D cell.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetCellSeed3D(int3 cell, ulong worldSeed = 0, NoiseType type = DefaultNoiseType)
        {
            var idx = (ulong)cell.x + Prime1Ul * (ulong)cell.y + Prime2Ul * (ulong)cell.z;
            return GetUInt64(idx, worldSeed, type);
        }

        #endregion

        #region Cellular / Worley

        /// <summary>
        /// Computes 2D cellular noise based on the input position, seed, frequency, and other custom parameters.
        /// This method evaluates the shortest and second shortest distances from the input position to the feature points
        /// within the cellular grid, returning the computed values along with additional information about the closest feature point and grid cell.
        /// </summary>
        /// <param name="p">The input 2D position as a float2 value, representing the point in space where the cellular noise is evaluated.</param>
        /// <param name="seed">An optional unsigned 64-bit integer used to initialize the random number generator for noise computation, adding determinism and variability to the output.</param>
        /// <param name="frequency">The scaling factor applied to the input position, controlling the density of features within the cellular grid. Default is 1.</param>
        /// <param name="jitter">The displacement factor ranging from 0 to 1, which determines how much feature points are offset from their grid positions. Higher values result in more irregular patterns. Default is 1.</param>
        /// <param name="metric">Specifies the distance metric used for computing distances between the input position and feature points (e.g., Euclidean, Manhattan, or Chebyshev). Default is Euclidean.</param>
        /// <param name="type">Defines the noise type, determining the underlying hash algorithm and the approach for mixing values. Default is the framework's default noise type.</param>
        /// <returns>An instance of the Cellular2DResult struct, containing the shortest distance, second shortest distance, closest feature point's position, and grid cell coordinates of the closest feature point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Cellular2DResult Cellular2D(
            float2 p, ulong seed = 0, float frequency = 1f, float jitter = 1f,
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
                var cxU = (ulong)cx;
                var cyU = (ulong)cy;

                var h0 = Get2DNoise(cxU, cyU, seed, type);
                var h1 = Get2DNoise(cxU + Prime1Ul, cyU + Prime2Ul, seed, type);

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
        /// Generates cellular noise in 3D space using the specified position, seed, frequency, and noise parameters.
        /// </summary>
        /// <param name="p">The 3D position at which to evaluate the cellular noise.</param>
        /// <param name="seed">A 64-bit unsigned integer seed used to introduce variability in the generated noise.</param>
        /// <param name="frequency">The frequency of the noise, determining its scale or repetition. Higher values result in finer details.</param>
        /// <param name="jitter">The amount of randomness applied to the base cell locations, affecting the "jitteriness" of the pattern.</param>
        /// <param name="metric">The distance metric used to compute cellular distances between feature points (e.g., Euclidean, Manhattan).</param>
        /// <param name="type">The noise generation type that specifies the algorithm for mixing and producing noise results.</param>
        /// <returns>An object containing information about the first and second closest distances to feature points, as well as the corresponding cell position and feature point location.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Cellular3DResult Cellular3D(
            float3 p, ulong seed = 0, float frequency = 1f, float jitter = 1f,
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
                var cxU = (ulong)cx;
                var cyU = (ulong)cy;
                var czU = (ulong)cz;

                var h0 = Get3DNoise(cxU, cyU, czU, seed, type);
                var h1 = Get3DNoise(cxU + Prime1Ul, cyU + Prime2Ul, czU, seed, type);
                var h2 = Get3DNoise(cxU, cyU + Prime1Ul, czU + Prime2Ul, seed, type);

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
        public static float CellularF1(float2 p, ulong seed = 0, float frequency = 1f, float jitter = 1f,
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
        public static float CellularF2MinusF1(float2 p, ulong seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean, NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return r.F2 - r.F1;
        }

        #endregion

        #region Cellular / Worley – High-Level (Wormy / Blocky)

        /// <summary>
        /// Generates a 2D Worley noise value based on the provided parameters, with additional control over frequency, jitter, and edge width.
        /// </summary>
        /// <param name="p">The 2D position for which the noise value is computed.</param>
        /// <param name="seed">A 64-bit unsigned integer used to introduce randomness into the noise computation. Default is 0.</param>
        /// <param name="frequency">The frequency of the noise pattern, determining the scale or repetition of features. Default is 0.005f.</param>
        /// <param name="jitter">The level of randomness applied to cell points, affecting the distortion of the noise. Default is 1.0f.</param>
        /// <param name="metric">The distance metric used for computation, defining how distances between points are measured (e.g., Euclidean). Default is Euclidean.</param>
        /// <param name="edgeWidth">The width of the edges in the noise pattern, controlling the interpolation around cellular boundaries. Default is 0.1f.</param>
        /// <param name="type">The noise generation type, dictating the method and mixing algorithm used in the calculation. Default is MangledBitsBalancedMix.</param>
        /// <returns>A floating-point value representing the computed 2D Worley noise at the specified position and parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyWormy2D(float2 p, ulong seed = 0, float frequency = 0.005f, float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        /// <summary>
        /// Generates a 3D cellular noise value with additional control over frequency, jitter, edge width, and distance metric, customized for a "Worley Wormy" effect.
        /// </summary>
        /// <param name="p">The 3D coordinate at which to evaluate the noise.</param>
        /// <param name="seed">An optional seed for randomness to ensure variation in noise generation.</param>
        /// <param name="frequency">The frequency of the noise, controlling its scale and detail.</param>
        /// <param name="jitter">Controls the randomness of feature-point distribution within grid cells.</param>
        /// <param name="metric">The distance metric used to compute cell distances, influencing the noise pattern.</param>
        /// <param name="edgeWidth">The width of the transition between cells, affecting the sharpness or smoothness of edges.</param>
        /// <param name="type">Specifies the noise generation type, determining the algorithm used for computation.</param>
        /// <returns>A float value representing the 3D noise at the given coordinate, customized with the provided parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyWormy3D(float3 p, ulong seed = 0, float frequency = 0.006f, float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.12f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        /// <summary>
        /// Computes a 2D blocky Worley noise value based on the provided parameters. This function evaluates cellular noise and generates smooth transitions influenced by jitter and distance metrics.
        /// Defaults: Chebyshev, jitter=0.1, freq=0.01, edgeWidth=0.1.
        /// </summary>
        /// <param name="p">The 2D position in space where the noise value will be sampled.</param>
        /// <param name="seed">A 64-bit unsigned integer that introduces randomness to the noise generation process.</param>
        /// <param name="frequency">Determines the frequency or "scale" of the noise pattern. Smaller values spread noise features further apart.</param>
        /// <param name="jitter">Controls the variability of the cell center points, affecting the randomness of the generated pattern.</param>
        /// <param name="metric">Specifies the distance metric used to calculate the cellular noise, influencing the shape of the noise cells.</param>
        /// <param name="edgeWidth">Defines the thickness of the edges between the noise cells. Higher values result in thicker transitions.</param>
        /// <param name="type">The noise generation type, impacting internal computation and mixing operations for noise synthesis.</param>
        /// <returns>A floating-point value representing the computed 2D Worley blocky noise at the specified position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyBlocky2D(float2 p, ulong seed = 0, float frequency = 0.01f, float jitter = 0.1f,
            CellularDistance metric = CellularDistance.Chebyshev, float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType) =>
            CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        /// <summary>
        /// Computes a 3D blocky Worley noise value based on the given input parameters, allowing customization of frequency, jitter, distance metrics, and edge width.
        /// </summary>
        /// <param name="p">The 3D position at which the Worley noise is evaluated, represented as a float3.</param>
        /// <param name="seed">The base seed used for randomization, influencing variability in noise generation. Defaults to 0 if unspecified.</param>
        /// <param name="frequency">The frequency of the noise, controlling the scale and number of cells in 3D space. Defaults to 0.012f if unspecified.</param>
        /// <param name="jitter">The amount of randomness applied to cell points, ranging from 0 (no jitter) to higher values increasing randomness. Defaults to 0.1f.</param>
        /// <param name="metric">The cellular distance metric used when determining proximity, such as Euclidean, Manhattan, or Chebyshev. Defaults to Chebyshev.</param>
        /// <param name="edgeWidth">The width of the edges between cells, controlling transition smoothness between cell boundaries. Defaults to 0.12f.</param>
        /// <param name="type">The noise generation type that determines the mixing algorithm used during computation. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A floating-point value representing the computed Worley 3D noise, ranging within [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WorleyBlocky3D(float3 p, ulong seed = 0, float frequency = 0.012f, float jitter = 0.1f,
            CellularDistance metric = CellularDistance.Chebyshev, float edgeWidth = 0.12f,
            NoiseType type = DefaultNoiseType) => CellularEdgeSigned(p, seed, frequency, jitter, metric, edgeWidth, type);

        #endregion

        #region Cellular – Normalized F1 Convenience

        /// <summary>
        /// Computes a value between 0 and 1 based on cellular noise, given a 2D position, seed, frequency, jitter, distance metric, and noise type.
        /// Normalized F1 fill in [0,1] (2D)
        /// </summary>
        /// <param name="p">The 2D position point where the cellular noise is sampled.</param>
        /// <param name="seed">An optional 64-bit seed for randomness to diversify noise generation.</param>
        /// <param name="frequency">The frequency of the noise, adjusting the scale of the generated pattern.</param>
        /// <param name="jitter">The degree of irregularity added to the noise cell boundaries for variation.</param>
        /// <param name="metric">The distance metric used for calculating cellular distances, influencing the noise structure.</param>
        /// <param name="type">The noise generation type, determining the algorithm used for mixing noise values.</param>
        /// <returns>A floating-point value normalized between 0 and 1, representing the result of cellular noise computation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularFill01(
            float2 p, ulong seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return NormalizeF101(in r, metric, jitter);
        }

        /// <summary>
        /// Computes a 0-1 normalized cellular noise pattern at a specific 3D point based on the provided seed, frequency, jitter, distance metric, and noise generation type.
        /// Normalized F1 fill in [0,1] (3D)
        /// </summary>
        /// <param name="p">The 3D point at which the cellular noise is computed.</param>
        /// <param name="seed">A 64-bit unsigned integer seed value that introduces randomness into the pattern.</param>
        /// <param name="frequency">The frequency of the noise, which alters the scale and detail of the pattern. Higher values produce finer details.</param>
        /// <param name="jitter">The degree of irregularity or randomness in the cellular noise's grid cells. Ranges between 0 (no jitter) and 1 (maximum jitter).</param>
        /// <param name="metric">The distance metric used to compute cellular distances, such as Euclidean, Manhattan, or Chebyshev.</param>
        /// <param name="type">The noise generation type, controlling the underlying computation method for the noise values.</param>
        /// <returns>A floating-point value between 0 and 1 representing the normalized cellular noise result at the given 3D point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularFill01(
            float3 p, ulong seed = 0, float frequency = 1f, float jitter = 1f,
            CellularDistance metric = CellularDistance.Euclidean,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular3D(p, seed, frequency, jitter,metric, type);
            return NormalizeF101(in r, metric, jitter);
        }

        #endregion

        #region Cellular / Worley – Edge Masks (01 & Signed)

        /// <summary>
        /// Computes a value representing a cellular noise effect with a smooth edge transition between different cells, normalized to the range [0, 1].
        /// </summary>
        /// <param name="p">The 2D position at which to evaluate the cellular noise.</param>
        /// <param name="seed">A 64-bit unsigned integer seed to add randomness to the noise generation.</param>
        /// <param name="frequency">The frequency of the noise, controlling the scale and spacing of cells.</param>
        /// <param name="jitter">The amount of positional variation applied to the cell points, influencing the randomness of cell shapes.</param>
        /// <param name="metric">The distance calculation method used to evaluate the proximity between cell points, such as Euclidean or Manhattan.</param>
        /// <param name="edgeWidth">The width of the transition area between different cells, affecting the sharpness of the edges.</param>
        /// <param name="type">The noise generation type, specifying the underlying algorithm for computing noise values.</param>
        /// <returns>A floating-point value normalized to the range [0, 1], representing the computed cellular edge noise at the given position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdge01(float2 p, ulong seed = 0, float frequency = 0.005f, float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean, float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return CellularVein01(in r, edgeWidth);
        }

        /// <summary>
        /// Computes a smooth noise value representing a cellular pattern with a customizable edge effect, based on the provided position, seed, frequency, jitter, and edge parameters.
        /// </summary>
        /// <param name="p">The input 3D position in space used to sample the noise value.</param>
        /// <param name="seed">A 64-bit unsigned integer that introduces randomness to the generated noise. Defaults to 0.</param>
        /// <param name="frequency">A scalar value that scales the frequency of the cellular noise pattern. Higher values create more detailed patterns. Defaults to 0.006.</param>
        /// <param name="jitter">A factor controlling the randomness or displacement of cell centers. Higher values lead to more jittered patterns. Defaults to 0.9.</param>
        /// <param name="metric">The distance metric used to compute the cellular noise. Defaults to Euclidean.</param>
        /// <param name="edgeWidth">The width of the transition between cell edges and their interiors. Smaller values create sharper edges. Defaults to 0.12.</param>
        /// <param name="type">The noise generation type, specifying the algorithm or mixing strategy for noise computation. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A floating-point value in the range [0, 1] representing the computed cellular noise with the specified edge effect applied.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdge01(
            float3 p, ulong seed = 0,
            float frequency = 0.006f,
            float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean,
            float edgeWidth = 0.12f,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular3D(p, seed, frequency, jitter, metric, type);
            return CellularVein01(in r, edgeWidth);
        }

        /// <summary>
        /// Computes a value based on cellular noise and transforms it using an exponential decay function, providing a smoothly blended result between cellular distances.
        /// </summary>
        /// <param name="p">The 2D coordinate used to calculate the cellular noise at a specific position in space.</param>
        /// <param name="seed">A seed value used to introduce randomness to the cellular noise calculation. Defaults to 0.</param>
        /// <param name="frequency">The frequency of the cellular noise, determining the scale of features. Defaults to 0.005.</param>
        /// <param name="jitter">The jitter factor controlling the variability in cell placements within the noise grid. Defaults to 1.0.</param>
        /// <param name="metric">The distance metric used to compute cell boundaries, such as Euclidean, Manhattan, or Chebyshev. Defaults to Euclidean.</param>
        /// <param name="k">A sharpness factor for the exponential decay applied to the cellular noise result. Defaults to 8.0.</param>
        /// <param name="type">The noise generation type defining the mixing and hashing algorithm for the noise computation. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A float value in the range [0, 1] representing the transformed cellular noise result, biased by the exponential function.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeExp01(
            float2 p, ulong seed = 0,
            float frequency = 0.005f,
            float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean,
            float k = 8f,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular2D(p, seed, frequency, jitter, metric, type);
            return CellularVeinExp01(in r, k);
        }

        /// <summary>
        /// Computes a non-linear cellular noise value in a clamped range of [0, 1], using an exponential vein contrast effect based on the input position and parameters.
        /// </summary>
        /// <param name="p">The 3D position vector used as input for the cellular noise computation.</param>
        /// <param name="seed">A 64-bit unsigned integer that introduces randomness into the noise calculation. Defaults to 0.</param>
        /// <param name="frequency">The frequency of the noise, scaling the input space. Defaults to 0.006f.</param>
        /// <param name="jitter">A value controlling the irregularity in the distance calculation of cellular features. Defaults to 0.9f.</param>
        /// <param name="metric">The distance metric used to compute cellular noise features, such as Euclidean, Manhattan, or Chebyshev. Defaults to CellularDistance.Euclidean.</param>
        /// <param name="k">A scalar influencing the exponential contrast of the cellular vein effect. Higher values sharpen the contrast. Defaults to 8f.</param>
        /// <param name="type">The noise generation method to determine the mixing algorithm for noise computation. Defaults to NoiseType.MangledBitsBalancedMix.</param>
        /// <returns>A clamped noise value in the range [0, 1] with non-linear vein structures based on the input parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeExp01(
            float3 p, ulong seed = 0,
            float frequency = 0.006f,
            float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean,
            float k = 8f,
            NoiseType type = DefaultNoiseType)
        {
            var r = Cellular3D(p, seed, frequency, jitter, metric, type);
            return CellularVeinExp01(in r, k);
        }

        /// <summary>
        /// Computes a signed cellular edge noise value in the range [-1, 1] for a given 2D point, based on the provided parameters such as seed, frequency, jitter, distance metric, edge width, and noise type.
        /// </summary>
        /// <param name="p">The 2D input point for which the noise value is computed.</param>
        /// <param name="seed">An optional 64-bit unsigned integer seed value used to introduce randomness into the noise computation.</param>
        /// <param name="frequency">A scaling factor that determines the frequency of the noise pattern. Higher values result in more dense patterns.</param>
        /// <param name="jitter">The amount of randomness applied to cell points for noise generation. Higher values create more irregular patterns.</param>
        /// <param name="metric">The distance metric used to calculate distances between points for cellular noise. Options include Euclidean, Manhattan, and Chebyshev.</param>
        /// <param name="edgeWidth">The normalized width of the cellular edges, controlling the transition area between cells.</param>
        /// <param name="type">The noise generation algorithm type, determining how input values are processed and mixed to produce the noise.</param>
        /// <returns>A signed floating-point value in the range [-1, 1] representing the computed cellular edge noise for the given 2D point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeSigned(
            float2 p, ulong seed = 0,
            float frequency = 0.005f,
            float jitter = 1.0f,
            CellularDistance metric = CellularDistance.Euclidean,
            float edgeWidth = 0.1f,
            NoiseType type = DefaultNoiseType)
        {
            return CellularEdge01(p, seed, frequency, jitter, metric, edgeWidth, type) * 2f - 1f;
        }

        /// <summary>
        /// Computes a signed cellular edge noise value based on the given 3D position, seed, frequency, jitter, distance metric, edge width, and noise generation type.
        /// The result is remapped to the range [-1, 1].
        /// </summary>
        /// <param name="p">The 3D point for which the cellular edge noise should be computed.</param>
        /// <param name="seed">A 64-bit unsigned integer that adds randomness to the noise computation. Defaults to 0.</param>
        /// <param name="frequency">Controls the frequency of the noise. Higher values result in more granular noise variation. Defaults to 0.006.</param>
        /// <param name="jitter">Adjusts the randomness of the cell point distribution. Higher values increase irregularity. Defaults to 0.9.</param>
        /// <param name="metric">Specifies the distance metric used for calculating cellular distances (e.g., Euclidean, Manhattan, Chebyshev). Defaults to Euclidean.</param>
        /// <param name="edgeWidth">Determines the blend width at the edges of cells. Smaller values create sharper edges. Defaults to 0.12.</param>
        /// <param name="type">Specifies the noise generation type, which determines the method and mixing algorithm used. Defaults to MangledBitsBalancedMix.</param>
        /// <returns>A signed floating-point noise value in the range [-1, 1] derived from cellular edge computation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularEdgeSigned(float3 p, ulong seed = 0, float frequency = 0.006f, float jitter = 0.9f,
            CellularDistance metric = CellularDistance.Euclidean,
            float edgeWidth = 0.12f,
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
        /// <param name="p">The 3D position in space where the noise is evaluated.</param>
        /// <param name="seed">A 64-bit unsigned integer used to initialize noise computation for added randomness. Default is 0.</param>
        /// <param name="octaves">The number of layers of fractal Brownian motion applied. Default is 6.</param>
        /// <param name="frequency">The base frequency of the noise pattern. Default is 1.</param>
        /// <param name="lacunarity">The scaling factor applied to the frequency for each octave. Default is 2.</param>
        /// <param name="gain">The amplitude reduction factor for each successive octave. Default is 0.5.</param>
        /// <param name="warp1Amp">The amplitude of the first domain warp. Default is 0.75.</param>
        /// <param name="warp1Freq">The frequency of the first domain warp. Default is 0.5.</param>
        /// <param name="warp1Oct">The number of octaves used for the first domain warp. Default is 2.</param>
        /// <param name="warp2Amp">The amplitude of the second domain warp. Default is 0.35.</param>
        /// <param name="warp2Freq">The frequency of the second domain warp. Default is 1.2.</param>
        /// <param name="warp2Oct">The number of octaves used for the second domain warp. Default is 2.</param>
        /// <param name="ridgeMix">The blend factor for mixing the ridged noise component with the primary noise. Default is 0.3.</param>
        /// <param name="type">The noise type used for all operations. Default is the predefined noise type.</param>
        /// <returns>A floating-point value representing the computed noise, clamped within the range [-1, 1].</returns>
        public static float UberNoise(float3 p, ulong seed = 0, int octaves = 6, float frequency = 1f,
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