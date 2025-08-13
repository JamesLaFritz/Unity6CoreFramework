using System.Runtime.CompilerServices;

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
        public static float ToNegOneToOne(uint value) => (value / (float)uint.MaxValue) * 2f - 1f;

        #endregion
        
        #region Noise Mixing Methods

        #region 32-bit

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

        #region Mixing Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MangledBitsShiftXOR(uint index, uint seed)
        {
            var mangled = index * GoldenRatio;
            mangled += seed;
            mangled ^= (mangled >> 8);
            mangled += MurmurHash3Final1;
            mangled ^= (mangled << 8);
            mangled *= MurmurHash3Final2;
            mangled ^= (mangled >> 8);
            return mangled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MangledBitsBalancedMix(uint index, uint seed)
        {
            var mangled = index ^ seed;
            mangled *= MurmurHash3Final1; // MurmurHash3 constant
            mangled ^= mangled >> 13;
            mangled *= MurmurHash3Final2; // MurmurHash3 constant
            mangled ^= mangled >> 16;
            return mangled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MangledBitsRotational(uint index, uint seed)
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
        public static uint ChaChaQuarterRoundAdvanced(uint index, uint seed)
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
        public static uint ChaChaQuarterRoundSimple(uint index, uint seed)
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
        public static void ChaChaQuarterRound(ref uint prime, ref uint key1, ref uint key2, ref uint input)
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

        public static void ChaCha20Setup()
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
        public static ulong MangledBitsShiftXOR(ulong index, ulong seed)
        {
            var mangled = index * GoldenRatioUl;
            mangled += seed;
            mangled ^= (mangled >> 17);
            mangled += MurmurHash3Final1Ul;
            mangled ^= (mangled << 21);
            mangled *= MurmurHash3Final2Ul;
            mangled ^= (mangled >> 13);
            return mangled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MangledBitsBalancedMix(ulong index, ulong seed)
        {
            var mangled = index ^ seed;
            mangled *= MurmurHash3Final1Ul;
            mangled ^= mangled >> 29;
            mangled *= MurmurHash3Final2Ul;
            mangled ^= mangled >> 32;
            return mangled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MangledBitsRotational(ulong index, ulong seed)
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
        public static ulong ChaChaQuarterAdvanced(ulong index, ulong seed)
        {
            var a = index ^ ChaChaPrime1;
            var b = seed ^ ChaChaPrime2;
            var c = index * ChaChaPrime2 + seed * ChaChaPrime3;
            var d = seed ^ index ^ ChaChaPrime4;

            a += b; d ^= a; d = RotateLeft(d, 32);
            c += d; b ^= c; b = RotateLeft(b, 24);
            a += b; d ^= a; d = RotateLeft(d, 16);
            c += d; b ^= c; b = RotateLeft(b, 7); // canonical ChaCha constant

            return a ^ b ^ c ^ d;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ChaChaQuarterRoundSimple(ulong index, ulong seed)
        {
            var a = index + ChaChaPrime1Ul;
            var b = seed + ChaChaPrime2Ul;
            var c = index ^ ChaChaPrime3Ul;
            var d = seed ^ ChaChaPrime4Ul;

            a += b; d ^= a; d = RotateLeft(d, 32);
            c += d; b ^= c; b = RotateLeft(b, 24);
            a += b; d ^= a; d = RotateLeft(d, 16);
            c += d; b ^= c; b = RotateLeft(b, 7);

            return a ^ b ^ c ^ d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChaChaQuarterRound(ref ulong prime, ref ulong key1, ref ulong key2, ref ulong input)
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
    }
}