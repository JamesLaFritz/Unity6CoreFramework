#region Header
// CellularExactnessTests.cs
#endregion

using NUnit.Framework;
using Unity.Mathematics;
using CoreFramework.Random;


namespace CoreFramework.Tests.Editor
{
    public class CellularExactnessTests
    {
        [Test]
        public void F1Zero_AtCellCenters_Euclidean2D()
        {
            const float freq = 1f;
            const float jitter = 0f;

            for (int y = -8; y <= 8; y++)
            for (int x = -8; x <= 8; x++)
            {
                // sample the center of the (x,y) cell
                var r = SquirrelNoise32Bit.Cellular2D(new float2(x + 0.5f, y + 0.5f),
                    0, freq, jitter);
                Assert.AreEqual(0f, r.F1, 1e-6f);
                // nearest neighbor center is 1 unit away along axes
                Assert.AreEqual(1f, r.F2, 1e-6f);
            }
        }

        [Test]
        public void F1Zero_AtCellCenters_Chebyshev3D()
        {
            const float freq = 1f;
            const float jitter = 0f;

            for (int z = -4; z <= 4; z++)
            for (int y = -4; y <= 4; y++)
            for (int x = -4; x <= 4; x++)
            {
                var r = SquirrelNoise32Bit.Cellular3D(new float3(x + 0.5f, y + 0.5f, z + 0.5f),
                    0, freq, jitter, HashBasedNoiseUtils.CellularDistance.Chebyshev);
                Assert.AreEqual(0f, r.F1, 1e-6f);
                // Chebyshev distance to adjacent center is 1
                Assert.AreEqual(1f, r.F2, 1e-6f);
            }
        }
        
        [Test]
        public void F1AtCorners_MatchesCenterOffset_Euclidean2D()
        {
            const float freq = 1f;
            const float jitter = 0f;
            float expected = math.sqrt(0.5f); // ≈0.70710678

            for (var y = -8; y <= 8; y++)
            for (var x = -8; x <= 8; x++)
            {
                var r = SquirrelNoise32Bit.Cellular2D(new float2(x, y),
                    0, freq, jitter, HashBasedNoiseUtils.CellularDistance.Euclidean);
                Assert.AreEqual(expected, r.F1, 1e-6f);
            }
        }

        [Test]
        public void F1AtCorners_MatchesCenterOffset_Chebyshev3D()
        {
            const float freq = 1f;
            const float jitter = 0f;
            const float expected = 0.5f;

            for (int z = -4; z <= 4; z++)
            for (int y = -4; y <= 4; y++)
            for (int x = -4; x <= 4; x++)
            {
                var r = SquirrelNoise32Bit.Cellular3D(new float3(x, y, z),
                    0, freq, jitter, HashBasedNoiseUtils.CellularDistance.Chebyshev);
                Assert.AreEqual(expected, r.F1, 1e-6f);
            }
        }
    }
}