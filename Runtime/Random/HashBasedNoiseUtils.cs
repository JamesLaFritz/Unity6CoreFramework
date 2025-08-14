using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace CoreFramework.Random
{
    public static class HashBasedNoiseUtils
    {
        #region Constants

        #region 32-Bit

        public const uint MurmurHash3Final1 = 0x85EBCA6B; // final mix constant 1
        public const uint MurmurHash3Final2 = 0xC2B2AE35; // final mix constant 2
        public const uint GoldenRatio = 0x9E3779B9; // 2^32 / golden ratio

        public const uint ChaChaPrime1 = 0x61707865; // "expa" (standard from ChaCha block constants)
        public const uint ChaChaPrime2 = 0x3320646E; // "nd 3"
        public const uint ChaChaPrime3 = 0x79622D32; // "2-by"
        public const uint ChaChaPrime4 = 0x6B206574; // "te k"

        public const int Prime1 = 198491317; // Large prime number with non-boring bits
        public const int Prime2 = 6542989; // Large prime number with non-boring bits
        public const int Prime3 = 357239; // Large prime number with non-boring bits


        #endregion

        #region 64-Bit

        public const ulong MurmurHash3Final1Ul = 0x85EBCA77C2B2AE63UL;
        public const ulong MurmurHash3Final2Ul = 0xC2B2AE3D27D4EB4FUL;
        public const ulong GoldenRatioUl = 0x9E3779B97F4A7C15UL;

        public const ulong ChaChaPrime1Ul = 0x6170786561707865UL; // "expaxpax"
        public const ulong ChaChaPrime2Ul = 0x3320646E6E20646EUL; // "nd 3nd 3"
        public const ulong ChaChaPrime3Ul = 0x79622D3279622D32UL; // "2-by2-by"
        public const ulong ChaChaPrime4Ul = 0x6B2065746B206574UL; // "te kte k"

        #endregion

        #endregion

        #region Float Conversion Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToZeroToOne(uint value) => value / (float)uint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToNegOneToOne(uint value) => value / (float)uint.MaxValue * 2f - 1f;

        #endregion

        #region Noise Mixing Methods

        #region 32-bit

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

        #region Mixing Methods

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int offset)
            => (value << offset) | (value >> (64 - offset));

        #endregion

        #region Mixing Methods

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ChaChaQuarterAdvanced(ulong index, ulong seed)
        {
            var a = index ^ ChaChaPrime1;
            var b = seed ^ ChaChaPrime2;
            var c = index * ChaChaPrime2 + seed * ChaChaPrime3;
            var d = seed ^ index ^ ChaChaPrime4;

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

    }
}