#region Header
// SecureNoise64RNG.cs
// Author: James LaFritz
// Description: A deterministic, seed-based random number generator using SecureNoise64.
// Implements IRandomFunction<ulong, ulong> and provides utility accessors for common
// random operations (e.g., Vector generation, ranges, rotations) using modular helpers.
#endregion

using System;
using System.Runtime.CompilerServices;
using CoreFramework.Mathematics;
using Unity.Mathematics;

namespace CoreFramework.Random
{
    /// <summary>
    /// A random number generator based on SecureNoise64 that implements IRandomFunction
    /// for sequential use with full utility support.
    /// </summary>
    public struct SquirrelNoise64Rng : IRandomFunction<ulong, ulong>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SquirrelNoise64Rng(ulong seed, NoiseType type = NoiseType.ChaChaQuarterRoundSimple, ulong position = 0)
        {
            Seed = seed;
            Position = position;
            Type = type;
        }

        #region IRandomFunction<ulong, ulong>

        /// <summary>
        /// Defines the type of noise generation algorithm used in the random number generator.
        /// This property determines the specific noise variation technique applied, such as
        /// mangled bits or ChaCha quarter round, influencing the randomness characteristics.
        /// </summary>
        public NoiseType Type { get; set; }

        #region SRand-like (seed-related) methods

        /// <summary>
        /// Represents the seed value used to initialize the random number generator.
        /// Serves as the basis for ensuring deterministic random value generation,
        /// allowing reproducible sequences of random numbers.
        /// </summary>
        public ulong Seed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        /// <summary>
        /// Tracks the current position or state within the sequence of generated random values.
        /// Used to ensure deterministic progression of values based on the current state.
        /// </summary>
        public ulong Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        /// <summary>
        /// Resets the internal state of the random number generator to the specified seed and position.
        /// </summary>
        /// <param name="seed">The new seed value to initialize the random number generator.</param>
        /// <param name="position">The starting position for generating random values. Defaults to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetSeed(ulong seed, ulong position = 0)
        {
            Seed = seed;
            Position = position;
        }

        #endregion

        #region Rand-like (sequential random)

        /// <summary>
        /// Generates a uniformly random floating-point number within the specified range using Squirrel Noise.
        /// </summary>
        /// <param name="minInclusive">The lower bound of the range (inclusive).</param>
        /// <param name="maxInclusive">The upper bound of the range (inclusive).</param>
        /// <returns>A random float value between <paramref name="minInclusive"/> and <paramref name="maxInclusive"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Range(float minInclusive, float maxInclusive) => NoiseRngUtils.Range(minInclusive, maxInclusive, Value);

        /// <summary>
        /// Generates a random integer within a specified range using the Squirrel Noise RNG algorithm.
        /// </summary>
        /// <param name="minInclusive">The minimum value (inclusive) of the desired random integer range.</param>
        /// <param name="maxExclusive">The maximum value (exclusive) of the desired random integer range.</param>
        /// <returns>A random integer between <paramref name="minInclusive"/> (inclusive) and <paramref name="maxExclusive"/> (exclusive).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int minInclusive, int maxExclusive) => NoiseRngUtils.Range(minInclusive, maxExclusive, Uint32);

        /// <summary>
        /// Provides a floating-point value between 0 and 1, representing a normalized
        /// random value generated based on the current state of the random number generator.
        /// This value is derived using the associated random function and noise function.
        /// </summary>
        public float Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SquirrelNoise64Bit.Get1DNoise01(Position++, Seed);
        }

        /// <summary>
        /// Represents a random point within a unit sphere, where the sphere is centered
        /// at the origin and has a radius of 1. The distribution of points ensures
        /// uniformity within the sphere's volume, providing a balanced random sample.
        /// </summary>
        public float3 InsideUnitSphere 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var position = SquirrelNoise64Bit.Get1DNoise(Position, Seed);
                var radius = SquirrelNoise64Bit.Get3DNoise01(position, SquirrelNoise64Bit.Get2DNoise(position, Position, Seed), Position, Seed);
                return NoiseRngUtils.InsideUnitSphere(radius, OnUnitSphere);
            }
        }

        /// <summary>
        /// Generates a random two-dimensional point within the unit circle.
        /// The generated point's x and y coordinates are uniformly distributed,
        /// and the point lies inside or on the boundary of a circle with a radius of 1.
        /// </summary>
        public float2 InsideUnitCircle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => NoiseRngUtils.InsideUnitCircle(
                SquirrelNoise64Bit.Get2DNoise01(SquirrelNoise64Bit.Get1DNoise(Position, Seed), Position, Seed), Value);
        }

        /// <summary>
        /// Generates a random point uniformly distributed on the surface of a unit sphere.
        /// Useful for scenarios requiring random direction vectors, such as in simulations
        /// or graphical applications involving spherical distributions.
        /// </summary>
        public float3 OnUnitSphere
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var position = Position++;

                var positionZ = SquirrelNoise64Bit.Get1DNoise(position, Seed);
                var positionTheta = SquirrelNoise64Bit.Get2DNoise(position, positionZ, Seed);
                
                var z = SquirrelNoise64Bit.Get1DNoise01(positionZ, Seed) * 2f - 1f;
                var theta = SquirrelNoise64Bit.Get2DNoise01(positionZ, positionTheta, Seed);

                return NoiseRngUtils.OnUnitSphere(z, theta);
            }
        }

        /// <summary>
        /// Represents a randomly generated quaternion rotation.
        /// Provides an evenly distributed random rotation,
        /// useful for applications involving 3D rotational randomness such as
        /// randomized orientations or procedural generation.
        /// </summary>
        public quaternion Rotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var position = Position;
                var axis = OnUnitSphere;
                return NoiseRngUtils.Rotation(axis,
                    SquirrelNoise64Bit.Get4DNoise01(position, (ulong)axis.x, (ulong)axis.y, (ulong)axis.z, Seed));
            }
        }

        /// <summary>
        /// Generates a uniformly distributed random quaternion representing a random rotation.
        /// Useful for applications requiring unbiased rotational randomness, such as procedural orientation generation.
        /// </summary>
        public quaternion RotationUniform
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var position = Position;
                var positionX = SquirrelNoise64Bit.Get1DNoise(position, Seed);

                return NoiseRngUtils.RotationUniform(Value, SquirrelNoise64Bit.Get2DNoise01(position, positionX, Seed),
                    SquirrelNoise64Bit.Get3DNoise01(position, positionX, SquirrelNoise64Bit.Get2DNoise(position, positionX, Seed), Seed));
            }
        }

        public ulong2 Uint128
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SquirrelNoise64Bit.GetUInt128(Position++, Seed);
        }

        /// <summary>
        /// Generates a 64-bit unsigned integer value derived from the current state of the random number generator.
        /// Utilized for scenarios where high-precision randomness is required or larger numeric ranges are involved.
        /// </summary>
        public ulong Uint64 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SquirrelNoise64Bit.GetUInt64(Position++, Seed);
        }

        /// <summary>
        /// Provides a 32-bit unsigned integer value representing a random or pseudo-random result.
        /// Typically employed in generating random values for various computational purposes.
        /// Derived using a noise function based on the current state and position of the generator.
        /// </summary>
        public uint Uint32
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SquirrelNoise64Bit.Get1DNoise(Position++, Seed);
        }

        /// <summary>
        /// Generates a 16-bit unsigned integer (ushort) pseudo-random value.
        /// Useful for scenarios where a smaller random number with reduced memory
        /// footprint is sufficient. The value is derived from a higher precision
        /// random number by shifting its bits.
        /// </summary>
        public ushort Uint16 => NoiseRngUtils.Uint16(Uint32);

        /// <summary>
        /// Retrieves a randomly generated byte value derived from the internal state of the random number generator.
        /// This property utilizes the random function's mechanism to produce a uniformly distributed
        /// 8-bit unsigned integer value.
        /// </summary>
        public byte Byte
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => NoiseRngUtils.Byte(Uint32);
        }

        /// <summary>
        /// Represents a randomly generated 2D directional vector with normalized length.
        /// This property utilizes noise-based randomness to produce a direction that is
        /// evenly distributed within the unit circle in a 2D space.
        /// </summary>
        public float2 Direction2D
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new (SquirrelNoise64Bit.Get1DNoiseNeg1To1(Position, Seed),
                SquirrelNoise64Bit.Get2DNoiseNeg1To1(Position, SquirrelNoise64Bit.Get1DNoise(Position++, Seed), Seed));
        }

        /// <summary>
        /// Generates a random 2-dimensional vector with each component constrained within specified ranges.
        /// </summary>
        /// <param name="minX">The minimum value for the X component of the generated vector. Default is <see cref="float.MinValue"/>.</param>
        /// <param name="maxX">The maximum value for the X component of the generated vector. Default is <see cref="float.MaxValue"/>.</param>
        /// <param name="minY">The minimum value for the Y component of the generated vector. Default is <see cref="float.MinValue"/>.</param>
        /// <param name="maxY">The maximum value for the Y component of the generated vector. Default is <see cref="float.MaxValue"/>.</param>
        /// <returns>A <see cref="float2"/> representing a random 2D vector within the specified ranges.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 Float2(float minX = float.MinValue, float maxX = float.MaxValue, float minY = float.MinValue,
            float maxY = float.MaxValue)
        {
            var position = Position;
            var positionX = SquirrelNoise64Bit.Get1DNoise(position, Seed);
            return new float2(NoiseRngUtils.Range(minX, maxX, Value),
                NoiseRngUtils.Range(minY, maxY,
                    SquirrelNoise64Bit.Get2DNoise01(positionX, SquirrelNoise64Bit.Get2DNoise(position, positionX, Seed), Seed)));
        }

        /// <summary>
        /// Provides a random three-dimensional unit vector (float3) based on the current state of the random number generator.
        /// This allows the generation of a direction in 3D space uniformly distributed over the unit sphere.
        /// </summary>
        public float3 Direction3D
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var positionX = SquirrelNoise64Bit.Get1DNoise(Position, Seed);
                var positionY = SquirrelNoise64Bit.Get2DNoise(Position, positionX, Seed);
                return new float3(SquirrelNoise64Bit.Get1DNoiseNeg1To1(Position, Seed),
                    SquirrelNoise64Bit.Get2DNoiseNeg1To1(Position, positionX, Seed),
                    SquirrelNoise64Bit.Get3DNoiseNeg1To1(Position, positionX,
                        SquirrelNoise64Bit.Get3DNoise(Position, positionX, positionY, Seed)));
            }
        }

        /// <summary>
        /// Generates a random <see cref="float3"/> value with each component constrained within the defined ranges for X, Y, and Z axes.
        /// </summary>
        /// <param name="minX">The minimum value for the X-axis.</param>
        /// <param name="maxX">The maximum value for the X-axis.</param>
        /// <param name="minY">The minimum value for the Y-axis.</param>
        /// <param name="maxY">The maximum value for the Y-axis.</param>
        /// <param name="minZ">The minimum value for the Z-axis.</param>
        /// <param name="maxZ">The maximum value for the Z-axis.</param>
        /// <returns>A <see cref="float3"/> structure representing a position with randomly generated X, Y, and Z coordinates within the specified ranges.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3 Float3(float minX = float.MinValue, float maxX = float.MaxValue, float minY = float.MinValue,
            float maxY = float.MaxValue, float minZ = float.MinValue, float maxZ = float.MaxValue)
        {
            var position = Position;
            var positionX = SquirrelNoise64Bit.Get1DNoise(position, Seed);
            var positionY = SquirrelNoise64Bit.Get2DNoise(position, positionX, Seed);
            return new float3(NoiseRngUtils.Range(minX, maxX, Value),
                NoiseRngUtils.Range(minY, maxY, SquirrelNoise64Bit.Get2DNoise01(position, positionX, Seed)),
                NoiseRngUtils.Range(minZ, maxZ, SquirrelNoise64Bit.Get3DNoise01(position, positionX, positionY, Seed)));
        }

        /// <summary>
        /// Represents a randomly generated 4-dimensional directional vector.
        /// Provides a normalized float4 value, ensuring each direction is uniformly distributed
        /// across the 4D space.
        /// </summary>
        public float4 Direction4D
        {
            get
            {
                var positionX = SquirrelNoise64Bit.Get1DNoise(Position, Seed);
                var positionY = SquirrelNoise64Bit.Get2DNoise(Position, positionX, Seed);
                var positionZ = SquirrelNoise64Bit.Get3DNoise(Position, positionX, positionY, Seed);
                return new float4(SquirrelNoise64Bit.Get1DNoiseNeg1To1(Position, Seed),
                    SquirrelNoise64Bit.Get2DNoiseNeg1To1(Position, positionX, Seed),
                    SquirrelNoise64Bit.Get3DNoiseNeg1To1(Position, positionX, positionY, Seed),
                    SquirrelNoise64Bit.Get4DNoiseNeg1To1(positionX, positionY, positionZ,
                        SquirrelNoise64Bit.Get4DNoise(Position++, positionX, positionY, positionZ, Seed), Seed));
            }
        }

        /// <summary>
        /// Generates a 4D vector (float4), where each component is randomly selected within specified ranges.
        /// </summary>
        /// <param name="minX">The minimum value for the X component of the vector. Defaults to float.MinValue.</param>
        /// <param name="maxX">The maximum value for the X component of the vector. Defaults to float.MaxValue.</param>
        /// <param name="minY">The minimum value for the Y component of the vector. Defaults to float.MinValue.</param>
        /// <param name="maxY">The maximum value for the Y component of the vector. Defaults to float.MaxValue.</param>
        /// <param name="minZ">The minimum value for the Z component of the vector. Defaults to float.MinValue.</param>
        /// <param name="maxZ">The maximum value for the Z component of the vector. Defaults to float.MaxValue.</param>
        /// <param name="minW">The minimum value for the W component of the vector. Defaults to float.MinValue.</param>
        /// <param name="maxW">The maximum value for the W component of the vector. Defaults to float.MaxValue.</param>
        /// <returns>A float4 vector where each component is within the specified minimum and maximum range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4 Float4(float minX = float.MinValue, float maxX = float.MaxValue, float minY = float.MinValue,
            float maxY = float.MaxValue, float minZ = float.MinValue, float maxZ = float.MaxValue,
            float minW = float.MinValue, float maxW = float.MaxValue)
        {
            
            var position = Position;
            var positionX = SquirrelNoise64Bit.Get1DNoise(position, Seed);
            var positionY = SquirrelNoise64Bit.Get2DNoise(position, positionX, Seed);
            var positionZ = SquirrelNoise64Bit.Get3DNoise(position, positionX, positionY, Seed);
            return new float4(NoiseRngUtils.Range(minX, maxX, Value),
                NoiseRngUtils.Range(minY, maxY, SquirrelNoise64Bit.Get2DNoise01(position, positionX, Seed)),
                NoiseRngUtils.Range(minZ, maxZ, SquirrelNoise64Bit.Get3DNoise01(position, positionX, positionY, Seed)),
                NoiseRngUtils.Range(minW, maxW,
                    SquirrelNoise64Bit.Get4DNoise01(position, positionX, positionY, positionZ, Seed)));
        }

        
        /// <summary>
        /// Represents a boolean value generated based on a random function.
        /// Utilizes an underlying random noise function to produce true or false
        /// with an equal probability of approximately 50%.
        /// </summary>
        public bool Bool
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => NoiseRngUtils.Bool(Value);
        }

        /// <summary>
        /// Determines whether a chance-based event occurs based on the given probability.
        /// </summary>
        /// <param name="probabilityOfTrue">A floating-point value between 0.0 and 1.0 representing the probability of returning true.</param>
        /// <returns>True if the event occurs based on the provided probability, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Chance(float probabilityOfTrue) => NoiseRngUtils.Chance(Value, probabilityOfTrue);

        #endregion

        #endregion
    }
}