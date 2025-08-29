#region Header
// SignedMaskAndNegativeSpaceTests.cs
#endregion

using NUnit.Framework;
using Unity.Mathematics;
using CoreFramework.Random;
using static CoreFramework.Random.HashBasedNoiseUtils;

namespace CoreFramework.Tests.Editor
{
    public class SignedMaskAndNegativeSpaceTests
    {
        [Test]
        public void CellularEdgeSigned_NoNaNs_And_InRange()
        {
            for (var i = -64; i <= 64; i++)
            {
                var p = new float2(i * 0.73f, -i * 0.41f);
                var v = SquirrelNoise32Bit.CellularEdgeSigned(
                    p, 123u, frequency: 0.01f, jitter: 0.85f, metric: CellularDistance.Euclidean, edgeWidth: 0.1f);

                Assert.IsFalse(float.IsNaN(v) || float.IsInfinity(v));
                Assert.GreaterOrEqual(v, -1.2f);
                Assert.LessOrEqual(v, 1.2f);
            }
        }

        [Test]
        public void Perlin_NegativeBoundaries_NoJumps()
        {
            const float eps = 1e-3f;
            var worst = 0f;
            for (var k = -32; k <= 32; k++)
            {
                var x = -10 + k * 0.5f;
                var a = SquirrelNoise32Bit.Perlin(x - eps, -7.7f);
                var b = SquirrelNoise32Bit.Perlin(x + eps, -7.7f);
                worst = math.max(worst, math.abs(a - b));
            }
            Assert.LessOrEqual(worst, 1e-2f);
        }
    }
}