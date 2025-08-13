using System.Runtime.CompilerServices;
using Unity.Mathematics;

using static CoreFramework.Random.HashBasedNoiseUtils;
using static CoreFramework.Random.SquirrelNoise32Bit;
using static CoreFramework.Random.SquirrelNoise64Bit;


namespace CoreFramework.Random.Crypto
{
    public static class CryptoNoise
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 GenerateKey(uint seed) => GenerateKey((uint)GetUInt64(0, seed), seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 GenerateKey(uint position, uint seed) => GenerateKey(
            GetUInt64(position, seed, NoiseType.ChaChaQuarterRoundSimple),
            GetUInt64(position + Prime1 * position, seed, NoiseType.ChaChaQuarterRoundAdvanced));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 GenerateKey(ulong seed) =>
            GenerateKey(GetUInt64(0, seed, NoiseType.ChaChaQuarterRoundSimple), seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 GenerateKey(ulong position, ulong seed)
        {
            const NoiseType noiseType = NoiseType.ChaChaQuarterRoundAdvanced;
            return new uint4(Get1DNoise(position, seed, noiseType),
                Get2DNoise(position, position, seed, noiseType),
                Get3DNoise(position, position, position, seed, noiseType),
                Get4DNoise(position, position, position, position, seed, noiseType));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] GenerateKey64(uint seed) => GenerateKey64((uint)GetUInt64(0, seed), seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] GenerateKey64(uint position, uint seed) => GenerateKey64(
            GetUInt64(position, seed, NoiseType.ChaChaQuarterRoundSimple),
            GetUInt64(position + Prime1 * position, seed, NoiseType.ChaChaQuarterRoundAdvanced));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] GenerateKey64(ulong seed) =>
            GenerateKey64(GetUInt64(0, seed, NoiseType.ChaChaQuarterRoundSimple), seed);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong[] GenerateKey64(ulong position, ulong seed)
        {
            uint2 j = new uint2(0, 0);
            
            var noiseType = NoiseType.ChaChaQuarterRoundSimple;
            var positionX = GetUInt64(position, seed, NoiseType.ChaChaQuarterRoundSimple);
            var positionY = GetUInt64(position + Prime1 * positionX, seed, noiseType);
            var positionZ = GetUInt64(position + Prime1 * positionX + Prime2 * positionY, seed, noiseType);
            var positionW = GetUInt64(position + Prime1 * positionX + Prime2 * positionY + Prime3 * positionZ, seed, noiseType);
            
            noiseType = NoiseType.ChaChaQuarterRoundAdvanced;
            return new[]
            {
                GetUInt64(positionX, seed, noiseType), GetUInt64(positionY, seed, noiseType),
                GetUInt64(positionZ, seed, noiseType), GetUInt64(positionW, seed, noiseType)
            };
        }
    }
}