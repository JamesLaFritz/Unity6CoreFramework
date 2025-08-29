#region Header
// DomainWarpEquivalenceTests.cs
#endregion

using NUnit.Framework;
using Unity.Mathematics;
using CoreFramework.Random;

namespace CoreFramework.Tests.Editor
{
    public class DomainWarpEquivalenceTests
    {
        [Test]
        public void DomainWarpZeroAmp_Equals_FBm()
        {
            const uint seed = 42u;
            const int oct = 6;
            const float freq = 0.006f;

            for (var i = 0; i < 256; i++)
            {
                var p2 = new float2(i * 0.5f, 17.3f);
                var p = new float3(p2, 0);

                var a = SquirrelNoise32Bit.DomainWarp(p, seed,
                    warpAmplitude: 0f, warpFrequency: 0.5f, warpOctaves: 2,
                    warpLacunarity: 2f, warpGain: 0.5f,
                    valueOctaves: oct, valueFrequency: freq, valueGain: 0.5f, valueLacunarity: 2f);

                var b = SquirrelNoise32Bit.FBm(p, seed, oct, freq, 1f, 2f, 0.5f, normalize: true);

                Assert.AreEqual(b, a, 5e-6f);
            }
        }
    }
}