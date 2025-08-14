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
using Unity.Mathematics;

namespace CoreFramework.Random
{
    /// <summary>
    /// Provides high-entropy stateless deterministic noise using a ChaCha-inspired mixing function.
    /// </summary>
    public static class SquirrelNoise64Bit
    {
        #region Constants

        private const ulong Prime1 = 0xD6E8FEB86659FD93UL; // Used in xxHash64 (good entropy)
        private const ulong Prime2 = 0xC2B2AE3D27D4EB4FUL; // From MurmurHash3 / xxHash
        private const ulong Prime3 = 0x165667B19E3779F9UL; // Mix of golden ratio and irregular prime

        private const NoiseType DefaultNoiseType = NoiseType.MangledBitsBalancedMix; // Default noise type to use

        #endregion

        // ToDo: Add ulong2(GetUint128) and ulong4(Get256)
        // ToDo: possible ulong8(GetUint512), and ulong16(GetUint1024)

        /// <summary>
        /// Retrieves a 128-bit unsigned integer noise value based on the specified index, seed, and noise generation type.
        /// </summary>
        /// <param name="index">The index value used to compute the noise, influencing the generated noise's position or state.</param>
        /// <param name="seed">A 64-bit unsigned integer that adds randomness to the generated noise.</param>
        /// <param name="type">The noise generation type, determining the noise computation method and mixing function.</param>
        /// <returns>A 128-bit unsigned integer represented as a struct containing two 64-bit components of pseudo-random noise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 GetUInt128(ulong index, ulong seed, NoiseType type = DefaultNoiseType)
        {
            var lo = GetUInt64(index, seed, type);
            return new ulong2(GetUInt64(index + Prime1 * lo, seed, type), lo);
        }

        /// <summary>
        /// Retrieves a 64-bit unsigned integer noise value based on the specified index, seed, and noise generation type.
        /// </summary>
        /// <param name="index">The index value used to compute the noise.</param>
        /// <param name="seed">The seed value that adds randomness to the generated noise.</param>
        /// <param name="type">The noise generation type, which defines the noise computation method.</param>
        /// <returns>A 64-bit unsigned integer representing the computed noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetUInt64(ulong index, ulong seed, NoiseType type = DefaultNoiseType)
        {
            return type switch
            {
                NoiseType.MangledBits => HashBasedNoiseUtils.MangledBitsShiftXOR(index, seed),
                NoiseType.MangledBitsBalancedMix => HashBasedNoiseUtils.MangledBitsBalancedMix(index, seed),
                NoiseType.MangledBitsRotational => HashBasedNoiseUtils.MangledBitsRotational(index, seed),
                NoiseType.ChaChaQuarterRoundSimple => HashBasedNoiseUtils.ChaChaQuarterRoundSimple(index, seed),
                NoiseType.ChaChaQuarterRoundAdvanced => HashBasedNoiseUtils.ChaChaQuarterAdvanced(
                    index, seed),
                _ => HashBasedNoiseUtils.MangledBitsShiftXOR(index, seed)
            };
        }

        /// <summary>
        /// Generates a deterministic 1D noise value as a 32-bit unsigned integer based on the given index and seed.
        /// </summary>
        /// <param name="index">The input index for which the noise value is generated.</param>
        /// <param name="seed">The seed value used for generating the deterministic noise.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 32-bit unsigned integer representing the generated 1D noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get1DNoise(ulong index, ulong seed, NoiseType type = DefaultNoiseType) =>
            (uint)GetUInt64(index, seed, type);

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
            Get1DNoise(x + Prime1 * y, seed, type);

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
            Get1DNoise(x + Prime1 * y + Prime2 * z, seed, type);

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
            Get1DNoise(x + Prime1 * y + Prime2 * z + Prime3 * w, seed, type);

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
            HashBasedNoiseUtils.ToZeroToOne(Get1DNoise(index, seed, type));

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
            HashBasedNoiseUtils.ToZeroToOne(Get2DNoise(x, y, seed, type));

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
            HashBasedNoiseUtils.ToZeroToOne(Get3DNoise(x, y, z, seed, type));

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
            HashBasedNoiseUtils.ToZeroToOne(Get4DNoise(x, y, z, w, seed, type));

        /// <summary>
        /// Generates a 1D noise value normalized to the range of -1 to 1.
        /// </summary>
        /// <param name="index">The index used to calculate the noise value.</param>
        /// <param name="seed">The seed value for ensuring deterministic noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point noise value in the range of -1 to 1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get1DNoiseNeg1To1(ulong index, ulong seed = 0, NoiseType type = DefaultNoiseType) =>
            HashBasedNoiseUtils.ToNegOneToOne(Get1DNoise(index, seed, type));

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
            HashBasedNoiseUtils.ToNegOneToOne(Get2DNoise(x, y, seed, type));

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
            HashBasedNoiseUtils.ToNegOneToOne(Get3DNoise(x, y, z, seed, type));

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
            HashBasedNoiseUtils.ToNegOneToOne(Get4DNoise(x, y, z, w, seed, type));

        #endregion

        #region Perlin

        /// <summary>
        /// Generates a 2D Perlin noise value based on the input coordinates, a seed, and the specified noise generation type.
        /// </summary>
        /// <param name="x">The x-coordinate of the sample point in the 2D noise field.</param>
        /// <param name="y">The y-coordinate of the sample point in the 2D noise field.</param>
        /// <param name="seed">An optional 64-bit unsigned integer seed providing additional randomness to the noise output. Defaults to 0.</param>
        /// <param name="type">The type of noise generation algorithm, affecting the computation method. Defaults to the system's default noise type.</param>
        /// <returns>A single-precision floating-point value representing the generated Perlin noise at the specified coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Perlin(float x, float y, ulong seed = 0, NoiseType type = DefaultNoiseType)
        {
            // Use double for inputs if you prefer; cast to float for core math
            var xi = (ulong)math.floor(x);
            var yi = (ulong)math.floor(y);
            var xf = x - xi;
            var yf = y - yi;

            // Corner hashes (reuse your integer noise to get stable corner IDs)
            var h00 = Get2DNoise(xi, yi, seed, type);
            var h10 = Get2DNoise(xi + 1UL, yi, seed, type);
            var h01 = Get2DNoise(xi, yi + 1UL, seed, type);
            var h11 = Get2DNoise(xi + 1UL, yi + 1UL, seed, type);

            return HashBasedNoiseUtils.Perlin(xf, yf, h00, h10, h01, h11);
        }

        /// <summary>
        /// Computes a 3D Perlin noise value for the given spatial coordinates, seed, and noise type.
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
            var xf = x - xi;
            var yf = y - yi;
            var zf = z - zi;

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

    }
}