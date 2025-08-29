#region Header
// PerlinContinuityTests.cs
#endregion

using NUnit.Framework;
using Unity.Mathematics;
using CoreFramework.Random;

namespace CoreFramework.Tests.Editor
{
    public class PerlinContinuityTests
    {
        private const uint Seed = 123u;
        private const float Eps = 1e-3f;

        [Test]
        public void Perlin2D_NoJumpsAcrossIntegerGrid()
        {
            var worst = 0f;
            for (var yi = -32; yi <= 32; yi++)
            for (var xi = -32; xi <= 32; xi++)
            {
                // Check vertical boundary at x = xi
                var vL = SquirrelNoise32Bit.Perlin(xi - Eps, yi + 0.37f, Seed);
                var vR = SquirrelNoise32Bit.Perlin(xi + Eps, yi + 0.37f, Seed);
                worst = math.max(worst, math.abs(vL - vR));

                // Check horizontal boundary at y = yi
                var vB = SquirrelNoise32Bit.Perlin(xi + 0.42f, yi - Eps, Seed);
                var vT = SquirrelNoise32Bit.Perlin(xi + 0.42f, yi + Eps, Seed);
                worst = math.max(worst, math.abs(vB - vT));
            }

            // Should be extremely small (only floating error). 1e-2 is roomy.
            Assert.LessOrEqual(worst, 1e-2f);
        }
    }
}