#region Header
// FBmNormalizationTests.cs
#endregion

using NUnit.Framework;
using Unity.Mathematics;
using CoreFramework.Random;

namespace CoreFramework.Tests.Editor
{
    public class FBmNormalizationTests
    {
        [TestCase(5, 0.5f)]
        [TestCase(7, 0.5f)]
        [TestCase(6, 0.6f)]
        public void NormalizedOutput_StaysWithinSignedBand(int oct, float gain)
        {
            const float freq = 0.008f;
            const uint seed = 7u;

            float min = float.PositiveInfinity, max = float.NegativeInfinity;
            for (var y = 0; y < 128; y++)
            for (var x = 0; x < 128; x++)
            {
                var v = SquirrelNoise32Bit.FBm(new float3(x * 3f, y * 3f, 0f), seed, oct, freq, 1f, 2f, gain, normalize: true);
                min = math.min(min, v);
                max = math.max(max, v);
            }

            Assert.GreaterOrEqual(min, -1.2f);
            Assert.LessOrEqual(max, 1.2f);
        }
    }
}