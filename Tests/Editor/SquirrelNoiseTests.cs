#region Header
// SquirrelNoiseTests.cs
// Author: You (+ helper scaffolding)
// Purpose: Burst-friendly grid sampling tests for SquirrelNoise32Bit/64Bit
// Notes: Validates ranges, determinism, Worley invariants, and provides a perf smoke test.
#endregion

using System.Diagnostics;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using CoreFramework.Random; // SquirrelNoise32Bit/64Bit + HashBasedNoiseUtils

namespace CoreFramework.Tests.Editor
{
    /// <summary>Which function a grid job should evaluate.</summary>
    internal enum NoiseEvalKind
    {
        Perlin2D,
        FBm3D,
        Billow3D,
        Ridge3D,
        DomainWarp3D,
        Uber3D,
        CellularEdge01_2D_Wormy,
        CellularEdge01_2D_Blocky
    }

    #region Burst Jobs

    /// <summary>
    /// Burst job that evaluates a 2D grid of samples (optionally lifting to 3D with z=0),
    /// writing results to a linear array for later stats/reduction on the main thread.
    /// </summary>
    [BurstCompile(FloatPrecision = FloatPrecision.Standard, FloatMode = FloatMode.Fast, CompileSynchronously = true)]
    internal struct EvaluateGrid2DJob : IJobParallelFor
    {
        #region Fields
        public int Width;
        public int Height;
        public float2 Origin;
        public float CellSize;

        // Common knobs
        public uint Seed32;
        public NoiseType NoiseType;
        public NoiseEvalKind Kind;

        // Fractal / warp knobs
        public int Octaves;
        public float Frequency;
        public float Lacunarity;
        public float Gain;
        public float Amplitude;

        // Cellular knobs
        public float Jitter;
        public float EdgeWidth;
        public HashBasedNoiseUtils.CellularDistance Metric;

        [WriteOnly] public NativeArray<float> Results;
        #endregion

        /// <summary>Evaluate the selected noise function at each grid point.</summary>
        public void Execute(int index)
        {
            var x = index % Width;
            var y = index / Width;
            var p = Origin + new float2(x, y) * CellSize;

            var v = Kind switch
            {
                NoiseEvalKind.Perlin2D => SquirrelNoise32Bit.Perlin(p.x, p.y, Seed32, NoiseType) // ~[-1,1]
                ,
                NoiseEvalKind.FBm3D => SquirrelNoise32Bit.FBm(new float3(p, 0f), Seed32, Octaves, Frequency, Amplitude,
                    Lacunarity, Gain, normalize: true, NoiseType),
                NoiseEvalKind.Billow3D => SquirrelNoise32Bit.Billow(new float3(p, 0f), Seed32, Octaves, Frequency,
                    Amplitude, Lacunarity, Gain, normalize: true, NoiseType),
                NoiseEvalKind.Ridge3D => SquirrelNoise32Bit.Ridge(new float3(p, 0f), Seed32, Octaves, Frequency,
                    Amplitude, Lacunarity, Gain, ridgeOffset: 1.0f, ridgeSharpness: 2.5f, normalize: true, NoiseType),
                NoiseEvalKind.DomainWarp3D => SquirrelNoise32Bit.DomainWarp(new float3(p, 0f), Seed32,
                    warpAmplitude: 0.6f, warpFrequency: Frequency * 0.5f, warpOctaves: math.max(1, Octaves - 3),
                    warpLacunarity: Lacunarity, warpGain: Gain, valueOctaves: Octaves, valueFrequency: Frequency,
                    valueGain: Gain, valueLacunarity: Lacunarity, NoiseType),
                NoiseEvalKind.Uber3D => SquirrelNoise32Bit.UberNoise(new float3(p, 0f), Seed32, octaves: Octaves,
                    frequency: Frequency, lacunarity: Lacunarity, gain: Gain, warp1Amp: 0.75f,
                    warp1Freq: Frequency * 0.5f, warp1Oct: 2, warp2Amp: 0.35f, warp2Freq: Frequency * 1.2f, warp2Oct: 2,
                    ridgeMix: 0.3f),
                NoiseEvalKind.CellularEdge01_2D_Wormy => SquirrelNoise32Bit.CellularEdge01(p, Seed32, Frequency, Jitter,
                    HashBasedNoiseUtils.CellularDistance.Euclidean, EdgeWidth, NoiseType) // [0,1]
                ,
                NoiseEvalKind.CellularEdge01_2D_Blocky => SquirrelNoise32Bit.CellularEdge01(p, Seed32, Frequency,
                    Jitter, Metric, EdgeWidth, NoiseType) // [0,1]
                ,
                _ => 0f
            };

            Results[index] = v;
        }
    }

    #endregion

    #region Stats Helpers

    /// <summary>Simple stats over a NativeArray after a job completes.</summary>
    internal static class GridStats
    {
        /// <summary>Compute min/max/mean.</summary>
        public static (float min, float max, float mean) MinMaxMean(NativeArray<float> data)
        {
            float min = float.PositiveInfinity, max = float.NegativeInfinity, sum = 0f;
            foreach (var v in data)
            {
                min = math.min(min, v);
                max = math.max(max, v);
                sum += v;
            }
            return (min, max, sum / math.max(1, data.Length));
        }
    }

    #endregion

    /// <summary>
    /// EditMode tests that smoke-test ranges, determinism, cellular invariants, and perf with Burst jobs.
    /// </summary>
    public class SquirrelNoiseEditModeTests
    {
        #region Fields (defaults)

        private const int W = 256;
        private const int H = 256;
        private static readonly float2 Origin = new float2(0, 0);
        private const float Cell = 1f;

        private const uint Seed32 = 123456789u;
        private const NoiseType DefaultType = NoiseType.MangledBitsBalancedMix;

        #endregion

        #region Helpers

        /// <summary>Create and schedule a grid evaluation job; returns results array (caller must Dispose)</summary>
        private static NativeArray<float> RunGridJob(NoiseEvalKind kind,
                                                     float frequency,
                                                     int octaves = 6,
                                                     float lacunarity = 2f,
                                                     float gain = 0.5f,
                                                     float amplitude = 1f,
                                                     float jitter = 1f,
                                                     float edgeWidth = 0.1f,
                                                     HashBasedNoiseUtils.CellularDistance metric = HashBasedNoiseUtils.CellularDistance.Euclidean)
        {
            var results = new NativeArray<float>(W * H, Allocator.TempJob);
            var job = new EvaluateGrid2DJob
            {
                Width = W,
                Height = H,
                Origin = Origin,
                CellSize = Cell,
                Seed32 = Seed32,
                NoiseType = DefaultType,
                Kind = kind,
                Octaves = octaves,
                Frequency = frequency,
                Lacunarity = lacunarity,
                Gain = gain,
                Amplitude = amplitude,
                Jitter = jitter,
                EdgeWidth = edgeWidth,
                Metric = metric,
                Results = results
            };
            var handle = job.Schedule(W * H, 256);
            handle.Complete();
            return results;
        }

        #endregion

        #region Tests — Ranges & Determinism

        [Test]
        public void Perlin2D_RangeAndDeterminism()
        {
            var a = RunGridJob(NoiseEvalKind.Perlin2D, frequency: 0.005f);
            var (minA, maxA, meanA) = GridStats.MinMaxMean(a);

            var b = RunGridJob(NoiseEvalKind.Perlin2D, frequency: 0.005f); // same inputs → same outputs
            var (minB, maxB, meanB) = GridStats.MinMaxMean(b);

            // Range (Perlin is ~[-1,1], allow safety headroom)
            Assert.LessOrEqual(maxA, 1.2f);
            Assert.GreaterOrEqual(minA, -1.2f);

            // Deterministic: aggregates must match closely
            Assert.AreEqual(minA, minB, 1e-6f);
            Assert.AreEqual(maxA, maxB, 1e-6f);
            Assert.AreEqual(meanA, meanB, 1e-6f);

            a.Dispose();
            b.Dispose();
        }

        [Test]
        public void Fractals_NormalizedRange()
        {
            using var fBm = RunGridJob(NoiseEvalKind.FBm3D, frequency: 0.006f, octaves: 6, lacunarity: 2f, gain: 0.5f);
            using var billow = RunGridJob(NoiseEvalKind.Billow3D, frequency: 0.006f, octaves: 6, lacunarity: 2f, gain: 0.6f);
            using var ridge = RunGridJob(NoiseEvalKind.Ridge3D, frequency: 0.009f, octaves: 5, lacunarity: 2f, gain: 0.5f);

            foreach (var arr in new[] { fBm, billow, ridge })
            {
                var (min, max, _) = GridStats.MinMaxMean(arr);
                Assert.LessOrEqual(max, 1.2f);
                Assert.GreaterOrEqual(min, -1.2f);
            }
        }

        [Test]
        public void Cellular_Wormy_Blocky_Ranges()
        {
            using var worm = RunGridJob(NoiseEvalKind.CellularEdge01_2D_Wormy, frequency: 0.01f, jitter: 1.0f, edgeWidth: 0.12f);
            using var blocky = RunGridJob(NoiseEvalKind.CellularEdge01_2D_Blocky, frequency: 0.012f, jitter: 0.1f, edgeWidth: 0.1f,
                                          metric: HashBasedNoiseUtils.CellularDistance.Chebyshev);

            // Edge01 is [0,1] by construction; allow tiny numerical slack
            var (minW, maxW, _) = GridStats.MinMaxMean(worm);
            var (minB, maxB, _) = GridStats.MinMaxMean(blocky);

            Assert.GreaterOrEqual(minW, -1e-4f);
            Assert.LessOrEqual(maxW, 1f + 1e-4f);
            Assert.GreaterOrEqual(minB, -1e-4f);
            Assert.LessOrEqual(maxB, 1f + 1e-4f);
        }

        [Test]
        public void DomainWarp_And_Uber_Range()
        {
            using var warp = RunGridJob(NoiseEvalKind.DomainWarp3D, frequency: 0.006f, octaves: 6);
            using var uber = RunGridJob(NoiseEvalKind.Uber3D, frequency: 0.006f, octaves: 6);

            var (minW, maxW, _) = GridStats.MinMaxMean(warp);
            var (minU, maxU, _) = GridStats.MinMaxMean(uber);

            Assert.LessOrEqual(maxW, 1.2f);
            Assert.GreaterOrEqual(minW, -1.2f);
            Assert.LessOrEqual(maxU, 1.2f);
            Assert.GreaterOrEqual(minU, -1.2f);
        }

        #endregion

        #region Tests — Cellular Invariants & Normalizers

        [Test]
        public void Cellular_F1F2_Invariants_And_NormalizedFill()
        {
            // Sample a small neighborhood and assert F2 >= F1 >= 0, and normalized fill in [0,1]
            var freq = 0.01f;
            var metric = HashBasedNoiseUtils.CellularDistance.Euclidean;
            const float jitter = 0.85f;

            for (var y = -4; y <= 4; y++)
            for (var x = -4; x <= 4; x++)
            {
                var p = new float2(x, y);
                var r = SquirrelNoise32Bit.Cellular2D(p, Seed32, frequency: freq, jitter: jitter, metric: metric);

                Assert.GreaterOrEqual(r.F1, 0f);
                Assert.GreaterOrEqual(r.F2, r.F1);

                var fill01 = HashBasedNoiseUtils.NormalizeF101(in r, metric, jitter);
                Assert.GreaterOrEqual(fill01, 0f - 1e-4f);
                Assert.LessOrEqual(fill01, 1f + 1e-4f);
            }
        }

        #endregion

        #region Test — 64-bit Smoke (determinism + range)

        [Test]
        public void Perlin3D_64Bit_Determinism_And_Range()
        {
            // Quick smoke using 64-bit backend on a modest grid (no job here to keep it short)
            const int w = 96;
            const int h = 96;
            const ulong seed = 0xDEADBEEFCAFEBABEUL;

            float min = float.PositiveInfinity, max = float.NegativeInfinity, sum = 0f;
            for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
            {
                var v = SquirrelNoise64Bit.Perlin(x * 0.01f, y * 0.01f, 0.0f, seed);
                min = math.min(min, v);
                max = math.max(max, v);
                sum += v;
            }

            Assert.LessOrEqual(max, 1.2f);
            Assert.GreaterOrEqual(min, -1.2f);

            // Determinism: sample a single point twice
            var a = SquirrelNoise64Bit.Perlin(12.34f, 56.78f, 9.1f, seed);
            var b = SquirrelNoise64Bit.Perlin(12.34f, 56.78f, 9.1f, seed);
            Assert.AreEqual(a, b, 0f);
        }

        #endregion

        #region Test — Performance (smoke)

        [Test]
        [Category("Performance")]
        public void FBm_Perf_Smoke_BurstJob()
        {
            // Not a hard perf assert (machines vary). We just report elapsed and sanity-check job ran.
            var sw = Stopwatch.StartNew();
            using var arr = RunGridJob(NoiseEvalKind.FBm3D, frequency: 0.006f, octaves: 7, lacunarity: 2f, gain: 0.5f);
            sw.Stop();

            var (min, max, mean) = GridStats.MinMaxMean(arr);
            Assert.LessOrEqual(max, 1.2f);
            Assert.GreaterOrEqual(min, -1.2f);

            TestContext.Out.WriteLine($"FBm grid {W}x{H} in {sw.Elapsed.TotalMilliseconds:F2} ms | min={min:F3} max={max:F3} mean={mean:F3}");
        }

        #endregion
    }
}
