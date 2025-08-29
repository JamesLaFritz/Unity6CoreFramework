#region Header
// SquirrelNoiseBitQualityTests.cs
// Purpose: Avalanche (diffusion) + low-bit bias for 32/64-bit Squirrel noise mixers.
#endregion

using NUnit.Framework;
using Unity.Mathematics;
using CoreFramework.Random;

namespace CoreFramework.Tests.Editor
{
    public class SquirrelNoiseBitQualityTests
    {
        private const int N = 8192;
        private const uint Seed32 = 0xA5A5A5A5u;
        private const ulong Seed64 = 0x9E3779B97F4A7C15UL;

        /// <summary>Average number of flipped bits between two uints.</summary>
        private static float AvgBitFlips32(uint[] a, uint[] b)
        {
            long sum = 0;
            for (var i = 0; i < a.Length; i++)
                sum += math.countbits(a[i] ^ b[i]);
            return (float)sum / a.Length;
        }

        /// <summary>Average number of flipped bits between two ulongs.</summary>
        private static float AvgBitFlips64(ulong[] a, ulong[] b)
        {
            long sum = 0;
            for (var i = 0; i < a.Length; i++)
                sum += math.countbits(a[i] ^ b[i]);
            return (float)sum / a.Length;
        }

        [Test]
        public void Avalanche_IndexDelta32()
        {
            var a = new uint[N];
            var b = new uint[N];
            for (var i = 0; i < N; i++)
            {
                a[i] = SquirrelNoise32Bit.Get1DNoise(i, Seed32);
                b[i] = SquirrelNoise32Bit.Get1DNoise(i + 1, Seed32); // minimal index delta
            }
            var flips = AvgBitFlips32(a, b);
            // Expect ~16; allow a wide band to avoid false positives on tiny N
            Assert.Greater(flips, 12f);
            Assert.Less(flips, 20f);
        }

        [Test]
        public void Avalanche_SeedDelta32()
        {
            var a = new uint[N];
            var b = new uint[N];
            var seedB = Seed32 ^ 0x00000001u; // single-bit toggle
            for (var i = 0; i < N; i++)
            {
                a[i] = SquirrelNoise32Bit.Get1DNoise(i, Seed32);
                b[i] = SquirrelNoise32Bit.Get1DNoise(i, seedB);
            }
            var flips = AvgBitFlips32(a, b);
            Assert.Greater(flips, 12f);
            Assert.Less(flips, 20f);
        }

        [Test]
        public void Avalanche_IndexDelta64()
        {
            var a = new ulong[N];
            var b = new ulong[N];
            for (ulong i = 0; i < N; i++)
            {
                a[i] = SquirrelNoise64Bit.GetUInt64(i, Seed64);
                b[i] = SquirrelNoise64Bit.GetUInt64(i + 1UL, Seed64);
            }
            var flips = AvgBitFlips64(a, b); // expect ~32
            Assert.Greater(flips, 24f);
            Assert.Less(flips, 40f);
        }

        [Test]
        public void LSB_Bias32()
        {
            var ones = 0;
            for (var i = 0; i < N; i++)
                ones += (int)(SquirrelNoise32Bit.Get1DNoise(i, Seed32) & 1u);
            var p = ones / (float)N; // frequency of 1s in LSB
            Assert.Greater(p, 0.46f);
            Assert.Less(p, 0.54f);
        }

        [Test]
        public void LSB_Bias64()
        {
            var ones = 0;
            for (ulong i = 0; i < N; i++)
                ones += (int)(SquirrelNoise64Bit.GetUInt64(i, Seed64) & 1UL);
            var p = ones / (float)N;
            Assert.Greater(p, 0.46f);
            Assert.Less(p, 0.54f);
        }
    }
}