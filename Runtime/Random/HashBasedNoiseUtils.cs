using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace CoreFramework.Random
{
    [BurstCompile]
    public static class HashBasedNoiseUtils
    {
        #region Constants

        /// <summary>
        /// A small incremental value used for numerical derivation calculations within hashing-based noise utilities.
        /// </summary>
        private const float DerivEps = 1e-3f;

        /// <summary>
        /// The reciprocal of the maximum value for a 32-bit unsigned integer, used for normalization.
        /// </summary>
        private const float InvUintMax = 1f / uint.MaxValue;

        #region 32-Bit

        /// <summary>
        /// A constant used as a final mixing value in MurmurHash3 algorithm computations.
        /// </summary>
        public const uint MurmurHash3Final1 = 0x85EBCA6B; // final mix constant 1

        /// <summary>
        /// A constant used in the final mixing step of the MurmurHash3 algorithm.
        /// </summary>
        public const uint MurmurHash3Final2 = 0xC2B2AE35; // final mix constant 2

        /// <summary>
        /// A constant value derived from the mathematical golden ratio, used as a mixing constant in hash-based noise functions.
        /// </summary>
        public const uint GoldenRatio = 0x9E3779B9; // 2^32 / golden ratio

        /// <summary>
        /// A constant representing the hexadecimal value 0x61707865 ("expa"), derived from the standard ChaCha block constants.
        /// Used as a prime value in hash-based noise generation and ChaCha operations.
        /// </summary>
        public const uint ChaChaPrime1 = 0x61707865; // "expa" (standard from ChaCha block constants)

        /// <summary>
        /// A prime constant derived from ChaCha block constants, used in ChaCha-based algorithms for noise generation and hashing operations.
        /// </summary>
        public const uint ChaChaPrime2 = 0x3320646E; // "nd 3"

        /// <summary>
        /// A predefined prime constant used in ChaCha-based number manipulation algorithms.
        /// Represents the hexadecimal value 0x79622D32 (interpreted as "2-by").
        /// </summary>
        public const uint ChaChaPrime3 = 0x79622D32; // "2-by"

        /// <summary>
        /// A predefined constant used in ChaCha-based noise generation operations.
        /// Represents the hexadecimal value 0x6B206574.
        /// </summary>
        public const uint ChaChaPrime4 = 0x6B206574; // "te k"

        /// <summary>
        /// A large prime number with non-boring bits used as a constant in hash-based noise calculations.
        /// </summary>
        public const int Prime1 = 198491317; // Large prime number with non-boring bits

        /// <summary>
        /// A large prime number with well-distributed bits, used in various noise generation algorithms for hashing and position computations.
        /// </summary>
        public const int Prime2 = 6542989; // Large prime number with non-boring bits

        /// <summary>
        /// A large prime number used for noise generation algorithms with well-distributed bits.
        /// </summary>
        public const int Prime3 = 357239; // Large prime number with non-boring bits


        #endregion

        #region 64-Bit

        /// <summary>
        /// A constant value used in the finalization step of the MurmurHash3 hash algorithm.
        /// </summary>
        public const ulong MurmurHash3Final1Ul = 0x85EBCA77C2B2AE63UL;

        /// <summary>
        /// A constant value used as part of the MurmurHash3 finalization steps.
        /// </summary>
        public const ulong MurmurHash3Final2Ul = 0xC2B2AE3D27D4EB4FUL;

        /// <summary>
        /// A constant representing the golden ratio in unsigned 64-bit format, commonly used for hashing and noise generation calculations.
        /// </summary>
        public const ulong GoldenRatioUl = 0x9E3779B97F4A7C15UL;

        /// <summary>
        /// A constant 64-bit unsigned integer used as one of the ChaCha cryptographic primes.
        /// Represents the hexadecimal value 0x6170786561707865UL.
        /// </summary>
        public const ulong ChaChaPrime1Ul = 0x6170786561707865UL; // "expaxpax"

        /// <summary>
        /// A constant 64-bit prime value used in ChaCha-based hash and noise computations.
        /// </summary>
        public const ulong ChaChaPrime2Ul = 0x3320646E6E20646EUL; // "nd 3nd 3"

        /// <summary>
        /// A constant used as a part of ChaCha-inspired hashing operations in the form of a 64-bit unsigned integer.
        /// The value is derived from the string "2-by2-by".
        /// </summary>
        public const ulong ChaChaPrime3Ul = 0x79622D3279622D32UL; // "2-by2-by"

        /// <summary>
        /// A constant prime value used in ChaCha-based noise generation algorithms.
        /// </summary>
        public const ulong ChaChaPrime4Ul = 0x6B2065746B206574UL; // "te kte k"

        /// <summary>
        /// A 64-bit prime constant used in xxHash64 and related hashing techniques for ensuring good entropy.
        /// </summary>
        public const ulong Prime1Ul = 0xD6E8FEB86659FD93UL; // Used in xxHash64 (good entropy)

        /// <summary>
        /// A constant 64-bit prime number used for hashing in noise generation algorithms,
        /// particularly in MurmurHash3 and xxHash implementations.
        /// </summary>
        public const ulong Prime2Ul = 0xC2B2AE3D27D4EB4FUL; // From MurmurHash3 / xxHash

        /// <summary>
        /// A 64-bit prime constant used in hash-based noise calculations.
        /// Combines characteristics of the golden ratio and irregular prime to improve distribution.
        /// </summary>
        public const ulong Prime3Ul = 0x165667B19E3779F9UL;

        #endregion

        #region Simplex Core (2D / 3D, hash-callback-based)

        /// <summary>
        /// Constant value defined as (√3 - 1) / 2, used as a skew factor in the 2D Simplex noise algorithm.
        /// </summary>
        public static readonly float SimplexF2 = 0.366025403784438646763723f; // (√3 - 1)/2

        /// <summary>
        /// The unskewing factor used in 2D Simplex noise calculations.
        /// This constant is derived as (3 - √3) / 6 and helps transform simplex grid coordinates
        /// back to Euclidean space.
        /// </summary>
        public static readonly float SimplexG2 = 0.211324865405187117745425f; // (3 - √3)/6

        /// <summary>
        /// A constant used in 3D Simplex Noise calculations for skewing coordinates.
        /// </summary>
        public const float SimplexF3 = 1f / 3f;

        /// <summary>
        /// The unskew factor for 3D Simplex noise, used in the conversion process
        /// between space coordinates and skewed simplex grid coordinates.
        /// </summary>
        public const float SimplexG3 = 1f / 6f;

        #endregion

        #endregion

        #region Float Conversion Methods

        /// <summary>
        /// Converts an unsigned integer value to a floating-point value in the range [0, 1].
        /// </summary>
        /// <param name="value">Unsigned integer value to be converted.</param>
        /// <returns>Floating-point value in the range [0, 1] derived from the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToZeroToOne(uint value) => value * InvUintMax;

        /// <summary>
        /// Converts an unsigned integer value to a floating-point value in the range [-1, 1].
        /// </summary>
        /// <param name="value">Unsigned integer value to be converted.</param>
        /// <returns>Floating-point value in the range [-1, 1] derived from the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToNegOneToOne(uint value) => value * InvUintMax * 2f - 1f;

        #endregion

        #region Noise Mixing Methods

        #region 32-bit

        /// <summary>
        /// Rotates the bits of a 32-bit unsigned integer to the left by a specified offset.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer whose bits are to be rotated.</param>
        /// <param name="offset">The number of positions to rotate the bits to the left.</param>
        /// <returns>The result of the bitwise left rotation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

        #region Mixing Methods

        /// <summary>
        /// Generates a deterministically "mangled" unsigned integer value by applying a series of bitwise operations and transformations.
        /// </summary>
        /// <param name="index">The input index value used for generating the mangled result.</param>
        /// <param name="seed">The seed value used to introduce variability to the generated result.</param>
        /// <returns>An unsigned integer that is the result of the mangling operations.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint MangledBitsShiftXOR(uint index, uint seed)
        {
            var mangled = index * GoldenRatio;
            mangled += seed;
            mangled ^= mangled >> 8;
            mangled += MurmurHash3Final1;
            mangled ^= mangled << 8;
            mangled *= MurmurHash3Final2;
            mangled ^= mangled >> 8;
            return mangled;
        }

        /// <summary>
        /// Generates a hashed value for the given index and seed using a balanced mix approach.
        /// </summary>
        /// <param name="index">The input index used for hashing.</param>
        /// <param name="seed">The seed value to influence the hash generation.</param>
        /// <returns>A hashed unsigned integer value derived from the input index and seed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint MangledBitsBalancedMix(uint index, uint seed)
        {
            var mangled = index ^ seed;
            mangled *= MurmurHash3Final1; // MurmurHash3 constant
            mangled ^= mangled >> 13;
            mangled *= MurmurHash3Final2; // MurmurHash3 constant
            mangled ^= mangled >> 16;
            return mangled;
        }

        /// <summary>
        /// Generates a pseudo-random value by applying rotational manipulations to the index and seed.
        /// </summary>
        /// <param name="index">The input index for generating the pseudo-random value.</param>
        /// <param name="seed">The seed value used to influence the randomness of the output.</param>
        /// <returns>A pseudo-random 32-bit unsigned integer value derived from the input index and seed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint MangledBitsRotational(uint index, uint seed)
        {
            var mangled = index * GoldenRatio; // golden ratio constant
            mangled += seed;
            mangled = RotateLeft(mangled, 5) ^ mangled;
            mangled *= MurmurHash3Final2; // arbitrary large odd constant
            mangled = RotateLeft(mangled, 13) ^ mangled;
            mangled ^= mangled >> 16;
            return mangled;
        }

        /// <summary>
        /// Computes a processed 32-bit unsigned integer based on the ChaCha quarter-round algorithm, enhanced with additional
        /// mixing patterns, using the given index and seed as inputs.
        /// </summary>
        /// <param name="index">The input index value used for computation.</param>
        /// <param name="seed">The seed value used to add variability to the computation.</param>
        /// <returns>A 32-bit unsigned integer resulting from the advanced ChaCha quarter-round computation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ChaChaQuarterRoundAdvanced(uint index, uint seed)
        {
            // Mix input into 4 words for quarter-round style processing
            var a = index ^ ChaChaPrime1;
            var b = seed ^ ChaChaPrime2;
            var c = index * ChaChaPrime2 + seed * ChaChaPrime3;
            var d = seed ^ index ^ ChaChaPrime4;

            // Standard ChaCha quarter-round pattern (rotations: 16, 12, 8, 7)
            a += b;
            d ^= a;
            d = RotateLeft(d, 16);
            c += d;
            b ^= c;
            b = RotateLeft(b, 12);
            a += b;
            d ^= a;
            d = RotateLeft(d, 8);
            c += d;
            b ^= c;
            b = RotateLeft(b, 7);

            // Collapse output to single uint
            return a ^ b ^ c ^ d;
        }

        /// <summary>
        /// Performs a simplified ChaCha quarter-round operation on the given index and seed values.
        /// </summary>
        /// <param name="index">The index value used in the calculation, typically representing a noise coordinate.</param>
        /// <param name="seed">A seed value used to add variability to the computation.</param>
        /// <returns>A 32-bit unsigned integer result derived from the ChaCha quarter-round process.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ChaChaQuarterRoundSimple(uint index, uint seed)
        {
            // Mix index and seed with ChaCha constants
            var a = index ^ ChaChaPrime1;
            var b = seed ^ ChaChaPrime2;
            var c = index + ChaChaPrime3;
            var d = seed + ChaChaPrime4;

            // Standard ChaCha quarter-round pattern (rotations: 16, 12, 8, 7)
            a += b;
            d ^= a;
            d = RotateLeft(d, 16);
            c += d;
            b ^= c;
            b = RotateLeft(b, 12);
            a += b;
            d ^= a;
            d = RotateLeft(d, 8);
            c += d;
            b ^= c;
            b = RotateLeft(b, 7);

            // Collapse output to single uint
            return a ^ b ^ c ^ d;
        }

        /// <summary>
        /// Performs a ChaCha quarter round operation on the provided input values.
        /// </summary>
        /// <param name="prime">A reference to the prime value used in the operation.</param>
        /// <param name="key1">A reference to the first key used in the operation.</param>
        /// <param name="key2">A reference to the second key used in the operation.</param>
        /// <param name="input">A reference to the input value that will be modified during the operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ChaChaQuarterRound(ref uint prime, ref uint key1, ref uint key2, ref uint input)
        {
            prime += key1;
            input ^= prime;
            input = RotateLeft(input, 16);

            key2 += input;
            key1 ^= key2;
            key1 = RotateLeft(key1, 12);

            prime += key1;
            input ^= prime;
            input = RotateLeft(input, 8);

            key2 += input;
            key1 ^= key2;
            key1 = RotateLeft(key1, 7);
        }

        /// <summary>
        /// Configures the initial state for the ChaCha20 cryptographic algorithm, preparing it for operations.
        /// </summary>
        internal static void ChaCha20Setup()
        {
            //[ "expa" | "nd 3" | "2-by" | "te k" ]
            //[ key0 | key1 | key2 | key3 ]
            //[ key4 | key5 | key6 | key7 ]
            //[ ctr  | nonce0 | nonce1 | nonce2 ]
        }

        #endregion

        #endregion

        #region 64-bit

        #region Helpers

        /// <summary>
        /// Performs a left rotation of the bits in the given unsigned 32-bit integer value.
        /// </summary>
        /// <param name="value">The unsigned 32-bit integer value to be rotated.</param>
        /// <param name="offset">The number of bit positions to rotate left.</param>
        /// <returns>The resulting value after performing the left rotation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int offset)
            => (value << offset) | (value >> (64 - offset));

        #endregion

        #region Mixing Methods

        /// <summary>
        /// Computes a pseudo-randomized 64-bit unsigned integer using bitwise mangling
        /// operations on the input index and seed values.
        /// </summary>
        /// <param name="index">The input index value to be used for the calculation.</param>
        /// <param name="seed">The seed value used to introduce variability in the calculation.</param>
        /// <returns>A pseudo-randomized 64-bit unsigned integer derived from the input index and seed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong MangledBitsShiftXOR(ulong index, ulong seed)
        {
            var mangled = index * GoldenRatioUl;
            mangled += seed;
            mangled ^= mangled >> 17;
            mangled += MurmurHash3Final1Ul;
            mangled ^= mangled << 21;
            mangled *= MurmurHash3Final2Ul;
            mangled ^= mangled >> 13;
            return mangled;
        }

        /// <summary>
        /// Applies a balanced mixing strategy to mangle the provided index and seed using bitwise operations
        /// and multiplication with predefined constants for generating a pseudorandom value.
        /// </summary>
        /// <param name="index">The input index to be mangled.</param>
        /// <param name="seed">The seed value used for mixing with the index.</param>
        /// <returns>A mangled 64-bit unsigned integer value derived from the index and seed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong MangledBitsBalancedMix(ulong index, ulong seed)
        {
            var mangled = index ^ seed;
            mangled *= MurmurHash3Final1Ul;
            mangled ^= mangled >> 29;
            mangled *= MurmurHash3Final2Ul;
            mangled ^= mangled >> 32;
            return mangled;
        }

        /// <summary>
        /// Generates a 64-bit hash value using a rotational mixing and mangling approach.
        /// </summary>
        /// <param name="index">The input index used as part of the hash calculation.</param>
        /// <param name="seed">The seed value used to randomize the result.</param>
        /// <returns>A 64-bit hash value derived from the index and seed using rotational bit manipulation and mixing.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong MangledBitsRotational(ulong index, ulong seed)
        {
            var mangled = index * GoldenRatioUl;
            mangled += seed;
            mangled = RotateLeft(mangled, 17) ^ mangled;
            mangled *= MurmurHash3Final2Ul;
            mangled = RotateLeft(mangled, 41) ^ mangled;
            mangled ^= mangled >> 33;
            return mangled;
        }

        /// <summary>
        /// Generates a pseudo-random 64-bit unsigned integer using a modified ChaCha quarter round process.
        /// </summary>
        /// <param name="index">The index used in the ChaCha quarter round algorithm.</param>
        /// <param name="seed">The seed value used to introduce randomness to the algorithm.</param>
        /// <returns>A pseudo-random 64-bit unsigned integer derived from the input index and seed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ChaChaQuarterAdvanced(ulong index, ulong seed)
        {
            var a = index ^ ChaChaPrime1Ul;
            var b = seed ^ ChaChaPrime2Ul;
            var c = index * ChaChaPrime2Ul + seed * ChaChaPrime3Ul;
            var d = seed ^ index ^ ChaChaPrime4Ul;

            a += b;
            d ^= a;
            d = RotateLeft(d, 32);
            c += d;
            b ^= c;
            b = RotateLeft(b, 24);
            a += b;
            d ^= a;
            d = RotateLeft(d, 16);
            c += d;
            b ^= c;
            b = RotateLeft(b, 7); // canonical ChaCha constant

            return a ^ b ^ c ^ d;
        }

        /// <summary>
        /// Performs a simplified ChaCha quarter round operation on the provided index and seed values.
        /// Uses predefined constants and bitwise operations to generate a transformed 64-bit value.
        /// </summary>
        /// <param name="index">The 64-bit index value used in the transformation.</param>
        /// <param name="seed">The 64-bit seed value used as an additional input for the transformation.</param>
        /// <returns>A 64-bit unsigned integer result produced by the simplified ChaCha quarter round operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ChaChaQuarterRoundSimple(ulong index, ulong seed)
        {
            var a = index + ChaChaPrime1Ul;
            var b = seed + ChaChaPrime2Ul;
            var c = index ^ ChaChaPrime3Ul;
            var d = seed ^ ChaChaPrime4Ul;

            a += b;
            d ^= a;
            d = RotateLeft(d, 32);
            c += d;
            b ^= c;
            b = RotateLeft(b, 24);
            a += b;
            d ^= a;
            d = RotateLeft(d, 16);
            c += d;
            b ^= c;
            b = RotateLeft(b, 7);

            return a ^ b ^ c ^ d;
        }

        /// <summary>
        /// Performs a single ChaCha quarter round operation on the provided prime, keys, and input values.
        /// </summary>
        /// <param name="prime">Reference to the prime value to be modified during the operation.</param>
        /// <param name="key1">Reference to the first key value to be modified during the operation.</param>
        /// <param name="key2">Reference to the second key value to be modified during the operation.</param>
        /// <param name="input">Reference to the input value to be modified during the operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ChaChaQuarterRound(ref ulong prime, ref ulong key1, ref ulong key2, ref ulong input)
        {
            prime += key1;
            input ^= prime;
            input = RotateLeft(input, 16);

            key2 += input;
            key1 ^= key2;
            key1 = RotateLeft(key1, 12);

            prime += key1;
            input ^= prime;
            input = RotateLeft(input, 8);

            key2 += input;
            key1 ^= key2;
            key1 = RotateLeft(key1, 7);
        }

        #endregion

        #endregion

        #endregion

        #region Perlin Core (shared 32/64)

        #region Fade (scalar)

        /// <summary>
        /// Applies a smoothing function to a value to produce a smooth transition in the range [0, 1].
        /// Perlin's quintic fade curve: 6t^5 - 15t^4 + 10t^3.
        /// </summary>
        /// <param name="t">Fractional coordinate in the range [0, 1].</param>
        /// <returns>Smoothed value in the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Fade(float t)
        {
            // t*t*t*(t*(t*6 - 15) + 10) is branchless and Burst-friendly
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        #endregion

        #region Gradients

        /// <summary>Computes a 2D gradient value based on a corner hash and local offsets.</summary>
        /// <param name="hash">Hash for the corner (low bits used).</param>
        /// <param name="dx">Local x offset from the corner.</param>
        /// <param name="dy">Local y offset from the corner.</param>
        /// <returns>Calculated gradient value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Grad2(uint hash, float dx, float dy)
        {
            var h = (int)(hash & 7u);
            var u = h < 4 ? dx : dy;
            var v = h < 4 ? dy : dx;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        /// <summary>Computes a gradient value based on the hash and the given offsets.</summary>
        /// <param name="hash">Hash value used to determine the gradient vector.</param>
        /// <param name="dx">Offset along the x-axis from the corner.</param>
        /// <param name="dy">Offset along the y-axis from the corner.</param>
        /// <param name="dz">Offset along the z-axis from the corner.</param>
        /// <returns>The dot product of the gradient vector and the offset vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Grad3(uint hash, float dx, float dy, float dz)
        {
            var h = (int)(hash & 15u);
            var u = h < 8 ? dx : dy;
            var v = h < 4 ? dy : h is 12 or 14 ? dx : dz;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        #endregion

        #region Perlin2D/3D (corner‑hash driven)

        /// <summary>
        /// Corner‑hash driven Perlin 2D. Callers pass the four corner hashes and the local
        /// fractional offsets inside the cell. This keeps the only 32 vs 64 difference (how
        /// you generate the corner IDs) out of the core.
        /// </summary>
        /// <param name="xf">Fractional x in [0,1) relative to the cell’s lower corner.</param>
        /// <param name="yf">Fractional y in [0,1) relative to the cell’s lower corner.</param>
        /// <param name="h00">Hash at (0,0) corner.</param>
        /// <param name="h10">Hash at (1,0) corner.</param>
        /// <param name="h01">Hash at (0,1) corner.</param>
        /// <param name="h11">Hash at (1,1) corner.</param>
        /// <returns>Smooth 2D noise value in [-1,1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float Perlin(float xf, float yf, uint h00, uint h10, uint h01, uint h11)
        {
            var u = Fade(xf);
            var v = Fade(yf);

            var n00 = Grad2(h00, xf, yf);
            var n10 = Grad2(h10, xf - 1, yf);
            var n01 = Grad2(h01, xf, yf - 1);
            var n11 = Grad2(h11, xf - 1, yf - 1);

            var nx0 = math.lerp(n00, n10, u);
            var nx1 = math.lerp(n01, n11, u);
            return math.lerp(nx0, nx1, v);
        }

        /// <summary>Generates a 3D Perlin noise value based on hashed gradient coordinates and fractional positions.</summary>
        /// <param name="xf">Fractional x-coordinate in the range [0,1].</param>
        /// <param name="yf">Fractional y-coordinate in the range [0,1].</param>
        /// <param name="zf">Fractional z-coordinate in the range [0,1].</param>
        /// <param name="h000">Hash value for the gradient at the (0, 0, 0) corner of the unit cube.</param>
        /// <param name="h100">Hash value for the gradient at the (1, 0, 0) corner of the unit cube.</param>
        /// <param name="h010">Hash value for the gradient at the (0, 1, 0) corner of the unit cube.</param>
        /// <param name="h110">Hash value for the gradient at the (1, 1, 0) corner of the unit cube.</param>
        /// <param name="h001">Hash value for the gradient at the (0, 0, 1) corner of the unit cube.</param>
        /// <param name="h101">Hash value for the gradient at the (1, 0, 1) corner of the unit cube.</param>
        /// <param name="h011">Hash value for the gradient at the (0, 1, 1) corner of the unit cube.</param>
        /// <param name="h111">Hash value for the gradient at the (1, 1, 1) corner of the unit cube.</param>
        /// <returns>A smooth noise value in the range [-1,1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float Perlin(
            float xf, float yf, float zf,
            uint h000, uint h100, uint h010, uint h110,
            uint h001, uint h101, uint h011, uint h111)
        {
            var u = Fade(xf);
            var v = Fade(yf);
            var w = Fade(zf);

            var n000 = Grad3(h000, xf, yf, zf);
            var n100 = Grad3(h100, xf - 1, yf, zf);
            var n010 = Grad3(h010, xf, yf - 1, zf);
            var n110 = Grad3(h110, xf - 1, yf - 1, zf);

            var n001 = Grad3(h001, xf, yf, zf - 1);
            var n101 = Grad3(h101, xf - 1, yf, zf - 1);
            var n011 = Grad3(h011, xf, yf - 1, zf - 1);
            var n111 = Grad3(h111, xf - 1, yf - 1, zf - 1);

            var nx00 = math.lerp(n000, n100, u);
            var nx10 = math.lerp(n010, n110, u);
            var nx01 = math.lerp(n001, n101, u);
            var nx11 = math.lerp(n011, n111, u);

            var nxy0 = math.lerp(nx00, nx10, v);
            var nxy1 = math.lerp(nx01, nx11, v);

            return math.lerp(nxy0, nxy1, w);
        }

        #endregion

        #endregion

        #region Perlin – Analytic Derivatives

        /// <summary>Derivative of Perlin's quintic fade: 30t^4 − 60t^3 + 30t^2.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FadeDeriv(float t)
        {
            // 30 t^2 (t-1)^2
            return 30f * t * t * (t - 1f) * (t - 1f);
        }

        #region Gradient vectors (for analytic derivatives)

        /// <summary>Returns a 2D gradient unit-ish vector based on low bits of <paramref name="hash"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float2 Grad2Vec(uint hash)
        {
            return (hash & 7u) switch
            {
                0 => new float2(1f, 1f),
                1 => new float2(-1f, 1f),
                2 => new float2(1f, -1f),
                3 => new float2(-1f, -1f),
                4 => new float2(1f, 0f),
                5 => new float2(-1f, 0f),
                6 => new float2(0f, 1f),
                _ => new float2(0f, -1f)
            };
        }

        /// <summary>Returns a 3D gradient vector from 12 canonical directions.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float3 Grad3Vec(uint hash)
        {
            switch (hash % 12u)
            {
                case 0: return new float3(1, 1, 0);
                case 1: return new float3(-1, 1, 0);
                case 2: return new float3(1, -1, 0);
                case 3: return new float3(-1, -1, 0);
                case 4: return new float3(1, 0, 1);
                case 5: return new float3(-1, 0, 1);
                case 6: return new float3(1, 0, -1);
                case 7: return new float3(-1, 0, -1);
                case 8: return new float3(0, 1, 1);
                case 9: return new float3(0, -1, 1);
                case 10: return new float3(0, 1, -1);
                default: return new float3(0, -1, -1);
            }
        }

        #endregion

        /// <summary>
        /// Corner-hash Perlin 2D with analytic gradient. Inputs are *fractional* coords (0..1) inside cell
        /// and the four corner hashes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float PerlinWithDeriv(
            float xf, float yf, uint h00, uint h10, uint h01, uint h11, out float2 d)
        {
            var u = Fade(xf);
            var du = FadeDeriv(xf);
            var v = Fade(yf);
            var dv = FadeDeriv(yf);

            // Corner gradient vectors
            var g00 = Grad2Vec(h00);
            var g10 = Grad2Vec(h10);
            var g01 = Grad2Vec(h01);
            var g11 = Grad2Vec(h11);

            // Corner dot products
            var n00 = math.dot(g00, new float2(xf, yf));
            var n10 = math.dot(g10, new float2(xf - 1, yf));
            var n01 = math.dot(g01, new float2(xf, yf - 1));
            var n11 = math.dot(g11, new float2(xf - 1, yf - 1));

            // X lerps
            var nx0 = math.lerp(n00, n10, u);
            var nx1 = math.lerp(n01, n11, u);

            // Partial derivatives of the X lerps
            var dnx0dx = g00.x + du * (n10 - n00) + u * (g10.x - g00.x);
            var dnx1dx = g01.x + du * (n11 - n01) + u * (g11.x - g01.x);

            var dnx0dy = g00.y + u * (g10.y - g00.y);
            var dnx1dy = g01.y + u * (g11.y - g01.y);

            // Final Y lerp
            var n = math.lerp(nx0, nx1, v);

            // Derivatives
            var dndx = dnx0dx + v * (dnx1dx - dnx0dx); // dv/dx = 0
            var dndy = dnx0dy + dv * (nx1 - nx0) + v * (dnx1dy - dnx0dy);

            d = new float2(dndx, dndy);
            return n;
        }

        /// <summary>
        /// Corner-hash Perlin 3D with analytic gradient. Inputs are fractional coords and eight corner hashes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float PerlinWithDeriv(
            float xf, float yf, float zf,
            uint h000, uint h100, uint h010, uint h110,
            uint h001, uint h101, uint h011, uint h111,
            out float3 d)
        {
            var u = Fade(xf);
            var du = FadeDeriv(xf);
            var v = Fade(yf);
            var dv = FadeDeriv(yf);
            var w = Fade(zf);
            var dw = FadeDeriv(zf);

            // Gradient vectors
            var g000 = Grad3Vec(h000);
            var g100 = Grad3Vec(h100);
            var g010 = Grad3Vec(h010);
            var g110 = Grad3Vec(h110);
            var g001 = Grad3Vec(h001);
            var g101 = Grad3Vec(h101);
            var g011 = Grad3Vec(h011);
            var g111 = Grad3Vec(h111);

            // Corner dots
            var n000 = math.dot(g000, new float3(xf, yf, zf));
            var n100 = math.dot(g100, new float3(xf - 1, yf, zf));
            var n010 = math.dot(g010, new float3(xf, yf - 1, zf));
            var n110 = math.dot(g110, new float3(xf - 1, yf - 1, zf));
            var n001 = math.dot(g001, new float3(xf, yf, zf - 1));
            var n101 = math.dot(g101, new float3(xf - 1, yf, zf - 1));
            var n011 = math.dot(g011, new float3(xf, yf - 1, zf - 1));
            var n111 = math.dot(g111, new float3(xf - 1, yf - 1, zf - 1));

            // X lerps on the four Z slices
            var nx00 = math.lerp(n000, n100, u);
            var nx10 = math.lerp(n010, n110, u);
            var nx01 = math.lerp(n001, n101, u);
            var nx11 = math.lerp(n011, n111, u);

            // d/dx for those
            var dnx00dx = g000.x + du * (n100 - n000) + u * (g100.x - g000.x);
            var dnx10dx = g010.x + du * (n110 - n010) + u * (g110.x - g010.x);
            var dnx01dx = g001.x + du * (n101 - n001) + u * (g101.x - g001.x);
            var dnx11dx = g011.x + du * (n111 - n011) + u * (g111.x - g011.x);

            // d/dy for those (u depends on x only)
            var dnx00dy = g000.y + u * (g100.y - g000.y);
            var dnx10dy = g010.y + u * (g110.y - g010.y);
            var dnx01dy = g001.y + u * (g101.y - g001.y);
            var dnx11dy = g011.y + u * (g111.y - g011.y);

            // d/dz for those
            var dnx00dz = g000.z + u * (g100.z - g000.z);
            var dnx10dz = g010.z + u * (g110.z - g010.z);
            var dnx01dz = g001.z + u * (g101.z - g001.z);
            var dnx11dz = g011.z + u * (g111.z - g011.z);

            // Y lerps on the two Z slices
            var nxy0 = math.lerp(nx00, nx10, v);
            var nxy1 = math.lerp(nx01, nx11, v);

            // Their derivatives
            var dnxy0dx = dnx00dx + v * (dnx10dx - dnx00dx); // dv/dx = 0
            var dnxy1dx = dnx01dx + v * (dnx11dx - dnx01dx);

            var dnxy0dy = dnx00dy + dv * (nx10 - nx00) + v * (dnx10dy - dnx00dy);
            var dnxy1dy = dnx01dy + dv * (nx11 - nx01) + v * (dnx11dy - dnx01dy);

            var dnxy0dz = dnx00dz + v * (dnx10dz - dnx00dz);
            var dnxy1dz = dnx01dz + v * (dnx11dz - dnx01dz);

            // Final Z lerp
            var n = math.lerp(nxy0, nxy1, w);

            // Final derivatives
            var dndx = dnxy0dx + w * (dnxy1dx - dnxy0dx);
            var dndy = dnxy0dy + w * (dnxy1dy - dnxy0dy);
            var dndz = dnxy0dz + dw * (nxy1 - nxy0) + w * (dnxy1dz - dnxy0dz);

            d = new float3(dndx, dndy, dndz);
            return n;
        }

        #endregion

        #region Seed Mixing (per-octave reseed)

        /// <summary>Mix a 32-bit seed with an octave index.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint MixOctave(uint seed, int octave)
        {
            var s = seed + GoldenRatio * (uint)(octave + 1);
            // MurmurHash3 finalization-ish
            s ^= s >> 16;
            s *= 0x85EBCA6B;
            s ^= s >> 13;
            s *= 0xC2B2AE35;
            s ^= s >> 16;
            return s;
        }

        /// <summary>Mix a 64-bit seed with an octave index.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong MixOctave(ulong seed, int octave)
        {
            var s = seed + GoldenRatioUl * (ulong)(octave + 1);
            s ^= s >> 33;
            s *= 0xFF51AFD7ED558CCDUL;
            s ^= s >> 33;
            s *= 0xC4CEB9FE1A85EC53UL;
            s ^= s >> 33;
            return s;
        }

        #endregion

        #region Normalization helpers

        /// <summary>
        /// Max possible sum for geometric series of amplitudes with 'gain'.
        /// Useful to remap fBm-like outputs back to [-1,1] or [0,1] ranges.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float AmplitudeSum(int octaves, float amplitude, float gain)
        {
            // amplitude * (1 - gain^octaves) / (1 - gain)
            if (math.abs(1f - gain) < 1e-6f) return amplitude * octaves;
            return amplitude * (1f - math.pow(gain, octaves)) / (1f - gain);
        }

        #endregion

        #region Cellular / Worley shared types

        /// <summary>Distance metric used by Cellular/Worley noise.</summary>
        public enum CellularDistance
        {
            /// <summary>√(dx²+dy²+dz²)</summary>
            Euclidean,

            /// <summary>|dx|+|dy|(+|dz|)</summary>
            Manhattan,

            /// <summary>max(|dx|,|dy|,(|dz|))</summary>
            Chebyshev
        }

        /// <summary>Result for 2D Worley / Cellular evaluation.</summary>
        [BurstCompile]
        public readonly struct Cellular2DResult
        {
            /// <summary>Nearest feature distance.</summary>
            public readonly float F1;

            /// <summary>Second nearest feature distance.</summary>
            public readonly float F2;

            /// <summary>World-space feature position of F1.</summary>
            public readonly float2 Feature;

            /// <summary>Integer lattice cell that owns the F1 feature.</summary>
            public readonly int2 Cell;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Cellular2DResult(float f1, float f2, float2 feature, int2 cell)
            {
                F1 = f1;
                F2 = f2;
                Feature = feature;
                Cell = cell;
            }
        }

        /// <summary>Result for 3D Worley / Cellular evaluation.</summary>
        [BurstCompile]
        public readonly struct Cellular3DResult
        {
            public readonly float F1;
            public readonly float F2;
            public readonly float3 Feature;
            public readonly int3 Cell;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Cellular3DResult(float f1, float f2, float3 feature, int3 cell)
            {
                F1 = f1;
                F2 = f2;
                Feature = feature;
                Cell = cell;
            }
        }

        #endregion

        #region Cellular helpers (distance)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CellularDistance2(float2 d, CellularDistance metric)
        {
            var a = math.abs(d);
            return metric switch
            {
                CellularDistance.Manhattan => a.x + a.y,
                CellularDistance.Chebyshev => math.cmax(a),
                _ => math.length(d)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CellularDistance3(float3 d, CellularDistance metric)
        {
            var a = math.abs(d);
            return metric switch
            {
                CellularDistance.Manhattan => a.x + a.y + a.z,
                CellularDistance.Chebyshev => math.cmax(a),
                _ => math.length(d)
            };
        }

        #endregion

        #region Cellular Remaps (F1/F2 utilities)

        /// <summary>
        /// Vein/edge intensity from a Cellular result, normalized to [0,1].
        /// Uses 1 - saturate((F2 - F1) / edgeWidth) so the value is bright near borders.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularVein01(in Cellular2DResult r, float edgeWidth = 0.1f)
            => 1f - math.saturate((r.F2 - r.F1) / edgeWidth);

        /// <summary>
        /// Vein/edge intensity from a Cellular result, normalized to [0,1] (3D overload).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularVein01(in Cellular3DResult r, float edgeWidth = 0.12f)
            => 1f - math.saturate((r.F2 - r.F1) / edgeWidth);

        /// <summary>
        /// Exponential edge falloff for veins. Higher k produces thinner, sharper lines.
        /// Output in [0,1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularVeinExp01(in Cellular2DResult r, float k = 8f)
            => math.exp(-k * math.max(0f, r.F2 - r.F1));

        /// <summary>Exponential edge falloff (3D overload). Output in [0,1].</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CellularVeinExp01(in Cellular3DResult r, float k = 8f)
            => math.exp(-k * math.max(0f, r.F2 - r.F1));

        #endregion

        #region Cellular F1 Normalization (metric/jitter aware)

/*
 * Distances in Cellular cores are computed in "cell units"
 * (because we evaluate at p*frequency). That makes them frequency-independent.
 * We bound F1 by a conservative metric/jitter-dependent maximum so F1/max ∈ [0,1].
 * Use NormalizeF101 for a fill (0 near sites, 1 far), or 1-NormalizeF101 for a soft mask.
 */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float MaxCellularF1_2D(CellularDistance metric, float jitter)
        {
            // Half-extent scaling based on a jittered feature region: s = 0.5*(1+jitter)
            var s = 0.5f * (1f + jitter);
            return metric switch
            {
                CellularDistance.Manhattan => 2f * s, // L1 radius across 2 dims
                CellularDistance.Chebyshev => 1f * s, // L∞ radius
                _ /* Euclidean */ => math.SQRT2 * s // sqrt(2)*s
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float MaxCellularF1_3D(CellularDistance metric, float jitter)
        {
            var s = 0.5f * (1f + jitter);
            return metric switch
            {
                CellularDistance.Manhattan => 3f * s, // L1 radius across 3 dims
                CellularDistance.Chebyshev => 1f * s, // L∞ radius
                _ /* Euclidean */ => math.sqrt(3f) * s // sqrt(3)*s
            };
        }

        /// <summary>
        /// Normalizes F1 to [0,1] using a conservative metric/jitter bound (2D).
        /// 0 near feature sites, 1 at farthest plausible locations given <paramref name="jitter"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeF101(in Cellular2DResult r, CellularDistance metric, float jitter)
        {
            var m = MaxCellularF1_2D(metric, jitter);
            return (m > 0f) ? math.saturate(r.F1 / m) : 0f;
        }

        /// <summary>Normalizes F1 to [0,1] using a conservative metric/jitter bound (3D).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeF101(in Cellular3DResult r, CellularDistance metric, float jitter)
        {
            var m = MaxCellularF1_3D(metric, jitter);
            return (m > 0f) ? math.saturate(r.F1 / m) : 0f;
        }

        /// <summary>
        /// Convenience: inverted normalized F1 (soft "blob" mask) in [0,1].
        /// 1 near sites, 0 far from sites.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeF1Inv01(in Cellular2DResult r, CellularDistance metric, float jitter)
            => 1f - NormalizeF101(in r, metric, jitter);

        /// <summary>
        /// 3D overload of <see>
        /// <cref>NormalizeF1Inv01(CoreFramework.Random.HashBasedNoiseUtils.Cellular2DResult, CellularDistance, float)</cref>
        /// </see>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeF1Inv01(in Cellular3DResult r, CellularDistance metric, float jitter)
            => 1f - NormalizeF101(in r, metric, jitter);

        #endregion

    }
}