#region Header
// SecureNoise128.cs
// Author: James LaFritz
// Description: 128-bit secure noise function using a ChaCha-inspired quarter-round scramble.
#endregion

using System.Runtime.CompilerServices;
using CoreFramework.Mathematics;

namespace CoreFramework.Random
{
    /// <summary>
    /// Provides a collection of static methods for generating various noise-based values
    /// using secure and efficient 128-bit computations. The class supports one-dimensional
    /// through four-dimensional noise generation.
    /// </summary>
    public static class SquirrelNoise128
    {
        #region Constants
        private const ulong Prime1 = 0x9E3779B185EBCA87UL; // 2^64 / golden ratio
        private const ulong Prime2 = 0xC13FA9A902A6328FUL;
        private const ulong Prime3 = 0x91E10DA5C79E7B1DUL;

        /// <summary>
        /// Represents the default noise type used in the noise generation methods of the SecureNoise128 class.
        /// The default value is set to <see cref="NoiseType.ChaChaQuarterRoundSimple"/>.
        /// </summary>
        private const NoiseType DefaultNoiseType = NoiseType.ChaChaQuarterRoundSimple; // Default noise type to use

        #endregion

        #region 64/128/256/512/1024 Noise
        
        // ToDo: Add ulong4(Get256)
        // possible ulong8(GetUint512), and ulong16(GetUint1024)

        /// <summary>
        /// Generates a 128-bit pseudorandom noise value based on the provided parameters.
        /// </summary>
        /// <param name="index">The input 128-bit vector index used as part of the noise calculation.</param>
        /// <param name="seed">The input 128-bit vector seed that modifies the noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 128-bit pseudorandom noise value represented as a <see cref="ulong2"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 GetUInt128(ulong2 index, ulong2 seed, NoiseType type = DefaultNoiseType)
        {
            
            return type switch
            {
                //NoiseType.MangledBits => NoiseUtils.MangledBitsShiftXOR(index, seed),
                //NoiseType.MangledBitsBalancedMix => NoiseUtils.MangledBitsBalancedMix(index, seed),
                //NoiseType.MangledBitsRotational => NoiseUtils.MangledBitsRotational(index, seed),
                //NoiseType.ChaChaQuarterRound => NoiseUtils.ChaChaQuarterRound(index, seed),
                //NoiseType.ChaChaQuarterRoundCompactMixing1 => NoiseUtils.ChaChaQuarterRoundCompactMixing(index, seed),
                //NoiseType.ChaChaQuarterRoundCompactMixing2 => NoiseUtils.ChaChaQuarterRoundCompactMixing2(index, seed),
                _ => HashBasedNoiseUtils.MangledBitsShiftXOR((ulong)index, (ulong)seed)
            };
        }

        /// <summary>
        /// Generates a pseudo-random 64-bit unsigned integer based on the provided index and seed values.
        /// </summary>
        /// <param name="index">The index used for noise generation, represented as a 2D vector of unsigned 64-bit integers (ulong2).</param>
        /// <param name="seed">The seed value for noise generation, represented as a 2D vector of unsigned 64-bit integers (ulong2).</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A pseudo-randomly generated 64-bit unsigned integer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetUInt64(ulong2 index, ulong2 seed, NoiseType type = DefaultNoiseType)
        {
            var a = GetUInt128(index, seed, type);
            return a.x | a.y;
        }

        #endregion

        #region 1D/2D/3D/4D Noise (UInt32)

        /// <summary>
        /// Generates a 1D deterministic pseudo-random noise value based on an integer index and a seed value.
        /// </summary>
        /// <param name="index">The 1D index to generate the noise for.</param>
        /// <param name="seed">The seed value represented as a 2D unsigned integer vector for random noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 32-bit unsigned integer representing the noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get1DNoise(ulong2 index, ulong2 seed, NoiseType type = DefaultNoiseType)
            => (uint)GetUInt64(index, seed, type);

        /// <summary>
        /// Generates a 2D noise value based on the provided coordinates and seed.
        /// </summary>
        /// <param name="x">The X-coordinate used for noise generation.</param>
        /// <param name="y">The Y-coordinate used for noise generation.</param>
        /// <param name="seed">A 64-bit seed for deterministic noise generation, supplied as a <c>ulong2</c> value.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 32-bit unsigned integer representing the generated 2D noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get2DNoise(ulong2 x, ulong2 y, ulong2 seed, NoiseType type = DefaultNoiseType)
            => Get1DNoise(x + Prime1 * y, seed, type);

        /// <summary>
        /// Generates a 3D noise value based on the provided coordinates and seed.
        /// </summary>
        /// <param name="x">The x-coordinate represented as a 64-bit vector.</param>
        /// <param name="y">The y-coordinate represented as a 64-bit vector.</param>
        /// <param name="z">The z-coordinate represented as a 64-bit vector.</param>
        /// <param name="seed">A 64-bit vector used as a seed for the noise generation process.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 32-bit unsigned integer representing the generated 3D noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get3DNoise(ulong2 x, ulong2 y, ulong2 z, ulong2 seed, NoiseType type = DefaultNoiseType)
            => Get1DNoise(x + Prime1 * y + Prime2 * z, seed, type);

        /// <summary>
        /// Generates a 4D noise value based on the provided 4D input coordinates and seed.
        /// </summary>
        /// <param name="x">The 4D noise input along the x-axis, represented as a `ulong2`.</param>
        /// <param name="y">The 4D noise input along the y-axis, represented as a `ulong2`.</param>
        /// <param name="z">The 4D noise input along the z-axis, represented as a `ulong2`.</param>
        /// <param name="w">The 4D noise input along the w-axis, represented as a `ulong2`.</param>
        /// <param name="seed">A unique seed value, represented as a `ulong2`, used to influence the noise output.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 32-bit unsigned integer representing the calculated noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get4DNoise(ulong2 x, ulong2 y, ulong2 z, ulong2 w, ulong2 seed, NoiseType type = DefaultNoiseType)
            => Get1DNoise(x + Prime1 * y + Prime2 * z + Prime3 * w, seed, type);

        #endregion

        #region Float Conversion Helpers

        /// <summary>
        /// Generates a 1D noise value scaled to the range [0, 1], based on the given index and seed.
        /// </summary>
        /// <param name="index">A 2D vector representing the index coordinates of the noise function input.</param>
        /// <param name="seed">A 2D vector used as a seed for random noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A 1D noise value in the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get1DNoise01(ulong2 index, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToZeroToOne(Get1DNoise(index, seed, type));

        /// <summary>
        /// Generates a 2D noise value scaled to the range [0, 1], using the specified coordinates and seed.
        /// </summary>
        /// <param name="x">The 2D coordinate value representing the x-dimension component.</param>
        /// <param name="y">The 2D coordinate value representing the y-dimension component.</param>
        /// <param name="seed">The 2D seed value influencing the random noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point value in the range [0, 1] representing the generated noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get2DNoise01(ulong2 x, ulong2 y, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToZeroToOne(Get2DNoise(x, y, seed, type));

        /// <summary>
        /// Generates 3D noise as a normalized float value between 0 and 1 using the provided 2D unsigned integer coordinates and seed.
        /// </summary>
        /// <param name="x">The unsigned 2D integer representing the x-coordinate.</param>
        /// <param name="y">The unsigned 2D integer representing the y-coordinate.</param>
        /// <param name="z">The unsigned 2D integer representing the z-coordinate.</param>
        /// <param name="seed">The unsigned 2D integer used as the seed for noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A float value between 0 and 1 representing the generated 3D noise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get3DNoise01(ulong2 x, ulong2 y, ulong2 z, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToZeroToOne(Get3DNoise(x, y, z, seed, type));

        /// <summary>
        /// Generates a normalized 4D noise value within the range [0, 1] based on the given input coordinates and seed values.
        /// </summary>
        /// <param name="x">The first component of the 4D input coordinates, represented as a ulong2 structure.</param>
        /// <param name="y">The second component of the 4D input coordinates, represented as a ulong2 structure.</param>
        /// <param name="z">The third component of the 4D input coordinates, represented as a ulong2 structure.</param>
        /// <param name="w">The fourth component of the 4D input coordinates, represented as a ulong2 structure.</param>
        /// <param name="seed">The seed value, represented as a ulong2 structure, used to initialize the noise generation process.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point number representing the 4D noise value normalized to the range [0, 1].</returns>
        public static float Get4DNoise01(ulong2 x, ulong2 y, ulong2 z, ulong2 w, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToZeroToOne(Get4DNoise(x, y, z, w, seed, type));

        /// <summary>
        /// Generates a 1D noise value within the range [-1, 1] based on the specified index and seed values.
        /// </summary>
        /// <param name="index">The 2D index wrapped in an <c>ulong2</c> structure, representing the coordinates for the noise generation.</param>
        /// <param name="seed">The 2D seed wrapped in an <c>ulong2</c> structure, representing the random seed used for noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point noise value in the range [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get1DNoiseNeg1To1(ulong2 index, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToNegOneToOne(Get1DNoise(index, seed, type));

        /// <summary>
        /// Generates a 2D noise value within the range [-1, 1] using the specified coordinates and seed.
        /// </summary>
        /// <param name="x">The 2D coordinate value representing the x-dimension component.</param>
        /// <param name="y">The 2D coordinate value representing the y-dimension component.</param>
        /// <param name="seed">The 2D seed value influencing the random noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A floating-point value in the range [-1, 1] representing the generated noise value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get2DNoiseNeg1To1(ulong2 x, ulong2 y, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToNegOneToOne(Get2DNoise(x, y, seed, type));

        /// <summary>
        /// Generates a 3D noise value in the range of -1 to 1 using the provided coordinates and seed.
        /// </summary>
        /// <param name="x">The x-coordinate as a 64-bit unsigned integer.</param>
        /// <param name="y">The y-coordinate as a 64-bit unsigned integer.</param>
        /// <param name="z">The z-coordinate as a 64-bit unsigned integer.</param>
        /// <param name="seed">A 64-bit unsigned integer used as the random seed for noise generation.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <return>A 3D noise value in the range of -1 to 1.</return>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get3DNoiseNeg1To1(ulong2 x, ulong2 y, ulong2 z, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToNegOneToOne(Get3DNoise(x, y, z, seed, type));

        /// <summary>
        /// Generates a 4D noise value in the range [-1, 1] based on the provided 4D coordinates and seed value.
        /// </summary>
        /// <param name="x">The first 64-bit unsigned integer coordinate.</param>
        /// <param name="y">The second 64-bit unsigned integer coordinate.</param>
        /// <param name="z">The third 64-bit unsigned integer coordinate.</param>
        /// <param name="w">The fourth 64-bit unsigned integer coordinate.</param>
        /// <param name="seed">The seed value used to initialize the noise generation process.</param>
        /// <param name="type">The type of noise algorithm to apply, defined by the <see cref="NoiseType"/> enumeration.</param>
        /// <returns>A float representing the 4D noise value in the range [-1, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Get4DNoiseNeg1To1(ulong2 x, ulong2 y, ulong2 z, ulong2 w, ulong2 seed, NoiseType type = DefaultNoiseType)
            => HashBasedNoiseUtils.ToNegOneToOne(Get4DNoise(x, y, z, w, seed, type));

        #endregion
    }
}
