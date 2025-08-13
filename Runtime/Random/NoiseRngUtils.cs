#region Header
// RandomFunctionUtils64.cs
// Author: James LaFritz
// Description: Utility methods shared by all IRandomFunction RNGs.
// These helpers provide stateless math utilities for value generation.
#endregion

using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace CoreFramework.Random
{
    /// <summary>
    /// Provides utility functions for generating random values using random function
    /// and random noise function interfaces. This class includes methods for working
    /// with various data types and structures in random generation scenarios.
    /// </summary>
    public static class NoiseRngUtils
    {
        #region Rand-like (sequential random)
        
        /// <summary>
        /// Clamps and Remaps the specified value to the specified range.
        /// </summary>
        /// <param name="minInclusive">The minimum inclusive bound of the range.</param>
        /// <param name="maxInclusive">The maximum inclusive bound of the range.</param>
        /// <param name="value">The Value</param>
        /// <returns>A float representing a value within the specified range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Range(float minInclusive, float maxInclusive, float value)
        {
            // Swap if out of order
            if (minInclusive > maxInclusive)
                (minInclusive, maxInclusive) = (maxInclusive, minInclusive);

            if (math.abs(maxInclusive - minInclusive) < (double)math.max(
                    1E-06f * math.max(math.abs(minInclusive), math.abs(maxInclusive)), math.EPSILON * 8f))
                return minInclusive;

            return minInclusive + value * (maxInclusive - minInclusive);
        }

        /// <summary>
        /// Generates a uniformly random integer within the specified range using a random function and noise function.
        /// </summary>
        /// <param name="minInclusive">The minimum bound of the range (inclusive).</param>
        /// <param name="maxExclusive">The maximum bound of the range (exclusive).</param>
        /// <param name="value"></param>
        /// <returns>A uniformly random integer between <paramref name="minInclusive"/> and <paramref name="maxExclusive"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(int minInclusive, int maxExclusive, uint value)
        {
            // Ensure valid ordering
            if (minInclusive > maxExclusive)
                (minInclusive, maxExclusive) = (maxExclusive, minInclusive);

            if (minInclusive == maxExclusive)
                return minInclusive;

            var rangeSize = (uint)(maxExclusive - minInclusive + 1);
            return minInclusive + (int)(maxExclusive == 0 ? 0 : (uint)(((ulong)value * rangeSize) >> 32));
        }

        /// <summary>
        /// Generates a random point inside or on the surface of a unit sphere (sphere with radius 1.0).
        /// Use Marsaglia's method to generate a point on the sphere, then scale by radius cube root
        /// </summary>
        /// <returns>A float3 representing a random point inside or on the surface of a unit sphere.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 InsideUnitSphere(float radius, float3 onUnitSphere) => onUnitSphere * math.pow(radius, 1f / 3f);

        /// <summary>
        /// Generates a random 2D point inside a unit circle, maintaining a uniform distribution across its area.
        /// </summary>
        /// <returns>A 2D point as a <c>float2</c> structure, representing the coordinates inside the unit circle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 InsideUnitCircle( float angle, float radius)
        {
            angle = Range(0f, math.PI * 2f, angle);
            return new float2(math.cos(angle), math.sin(angle)) * math.sqrt(radius);
        }

        /// <summary>
        /// Generates a random point on the surface of a unit sphere.
        /// </summary>
        /// <returns>A vector representing a random point on the unit sphere surface.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 OnUnitSphere(float z, float theta)
        {
            z = z * 2f - 1f;
            var r = math.sqrt(1f - z * z);
            theta = Range(0f, 2f * math.PI, theta);
            var x = r * math.cos(theta);
            var y = r * math.sin(theta);
            return new float3(x, y, z);
        }

        /// <summary>
        /// Generates a random quaternion representing a rotation.
        /// </summary>
        /// <returns>A randomly generated quaternion representing a rotation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Rotation(float3 axis, float angle)
        {
            angle = Range(0f, math.PI * 2f, angle);
            return quaternion.AxisAngle(axis, angle);
        }

        /// <summary>
        /// Generates a quaternion representing a random 3D rotation with uniform distribution.
        /// </summary>
        /// <returns>A randomly generated quaternion with uniform distribution over 3D rotation space.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion RotationUniform(float u1, float u2, float u3)
        {
            // Uniform sampling on SO(3) using Ken Shoemake's method
            var sqrt1MinusU1 = math.sqrt(1f - u1);
            var sqrtU1 = math.sqrt(u1);

            var theta1 = 2f * math.PI * u2;
            var theta2 = 2f * math.PI * u3;

            var w = math.cos(theta2) * sqrtU1;
            var x = math.sin(theta1) * sqrt1MinusU1;
            var y = math.cos(theta1) * sqrt1MinusU1;
            var z = math.sin(theta2) * sqrtU1;

            return new quaternion(x, y, z, w);
        }

        /// <summary>
        /// A 16-bit unsigned integer value based on the provided unsigned integer value.
        /// </summary>
        /// <returns>A 16-bit unsigned integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Uint16(uint value) => (ushort)(value >> 16);

        /// <summary>
        /// Converts a random uint value into a byte.
        /// </summary>
        /// <returns>A byte value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Byte(uint value) => (byte)(value >> 24);

        /// <summary>
        /// Computes a boolean value based on a pseudo-random calculation.
        /// </summary>
        /// <returns>True if the pseudo-random value is less than or equal to 0.5; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Bool(float value) => value <= 0.5f;

        /// <summary>
        /// Determines whether a random event occurs based on the given probability.
        /// </summary>
        /// <returns>True if the randomly generated value falls within the probability range, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(float value, float probabilityOfTrue) => value < probabilityOfTrue;

        #endregion
    }
}