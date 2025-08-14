using CoreFramework.Mathematics;
using Unity.Mathematics;

namespace CoreFramework.Random
{
    /// <summary>
    /// Interface defining a random number generation system with customizable index and seed types.
    /// </summary>
    /// <typeparam name="TIndex">
    /// The type representing the current position or index of the generator. Must be a value type.
    /// </typeparam>
    /// <typeparam name="TSeed">
    /// The type representing the initial seed value for the generator. Must be a value type.
    /// </typeparam>
    public interface IRandomFunction<TIndex, TSeed> where TIndex : struct where TSeed : struct
    {
        #region SRand-like (seed-related) methods

        /// <summary>
        /// Represents the seed value used to initialize the random number generator.
        /// Serves as the basis for ensuring deterministic random value generation,
        /// allowing reproducible sequences of random numbers.
        /// </summary>
        public TSeed Seed { get; set; }

        /// <summary>
        /// Tracks the current position or state within the sequence of generated random values.
        /// Used to ensure deterministic progression of values based on the current state.
        /// </summary>
        public TIndex Position { get; set; }

        /// <summary>
        /// Resets the internal state of the random number generator to the specified seed and position.
        /// </summary>
        /// <param name="seed">The new seed value to initialize the random number generator.</param>
        /// <param name="position">The starting position for generating random values. Defaults to 0.</param>
        public void ResetSeed(TSeed seed, TIndex position);

        #endregion

        #region Rand-like (sequential random)

        /// <summary>
        /// Generates a uniformly random floating-point number within the specified range using Squirrel Noise.
        /// </summary>
        /// <param name="minInclusive">The lower bound of the range (inclusive).</param>
        /// <param name="maxInclusive">The upper bound of the range (inclusive).</param>
        /// <returns>A random float value between <paramref name="minInclusive"/> and <paramref name="maxInclusive"/>.</returns>
        public float Range(float minInclusive, float maxInclusive);

        /// <summary>
        /// Generates a random integer within a specified range using the Squirrel Noise RNG algorithm.
        /// </summary>
        /// <param name="minInclusive">The minimum value (inclusive) of the desired random integer range.</param>
        /// <param name="maxExclusive">The maximum value (exclusive) of the desired random integer range.</param>
        /// <returns>A random integer between <paramref name="minInclusive"/> (inclusive) and <paramref name="maxExclusive"/> (exclusive).</returns>
        public int Range(int minInclusive, int maxExclusive);

        /// <summary>
        /// Provides a floating-point value between 0 and 1, representing a normalized
        /// random value generated based on the current state of the random number generator.
        /// This value is derived using the associated random function and noise function.
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// Represents a random point within a unit sphere, where the sphere is centered
        /// at the origin and has a radius of 1. The distribution of points ensures
        /// uniformity within the sphere's volume, providing a balanced random sample.
        /// </summary>
        public float3 InsideUnitSphere { get; }

        /// <summary>
        /// Generates a random two-dimensional point within the unit circle.
        /// The generated point's x and y coordinates are uniformly distributed,
        /// and the point lies inside or on the boundary of a circle with a radius of 1.
        /// </summary>
        public float2 InsideUnitCircle { get; }

        /// <summary>
        /// Generates a random point uniformly distributed on the surface of a unit sphere.
        /// Useful for scenarios requiring random direction vectors, such as in simulations
        /// or graphical applications involving spherical distributions.
        /// </summary>
        public float3 OnUnitSphere { get; }

        /// <summary>
        /// Represents a randomly generated quaternion rotation.
        /// Provides an evenly distributed random rotation,
        /// useful for applications involving 3D rotational randomness such as
        /// randomized orientations or procedural generation.
        /// </summary>
        public quaternion Rotation { get; }

        /// <summary>
        /// Generates a uniformly distributed random quaternion representing a random rotation.
        /// Useful for applications requiring unbiased rotational randomness, such as procedural orientation generation.
        /// </summary>
        public quaternion RotationUniform { get; }
        
        // ToDo: add other Uint### types

        public ulong2 Uint128 { get; }

        /// <summary>
        /// Generates a 64-bit unsigned integer value derived from the current state of the random number generator.
        /// Utilized for scenarios where high-precision randomness is required or larger numeric ranges are involved.
        /// </summary>
        public ulong Uint64 { get; }

        /// <summary>
        /// Provides a 32-bit unsigned integer value representing a random or pseudo-random result.
        /// Typically employed in generating random values for various computational purposes.
        /// Derived using a noise function based on the current state and position of the generator.
        /// </summary>
        public uint Uint32 { get; }

        /// <summary>
        /// Generates a 16-bit unsigned integer (ushort) pseudo-random value.
        /// Useful for scenarios where a smaller random number with reduced memory
        /// footprint is sufficient. The value is derived from a higher precision
        /// random number by shifting its bits.
        /// </summary>
        public ushort Uint16 { get; }

        /// <summary>
        /// Retrieves a randomly generated byte value derived from the internal state of the random number generator.
        /// This property utilizes the random function's mechanism to produce a uniformly distributed
        /// 8-bit unsigned integer value.
        /// </summary>
        public byte Byte { get; }

        /// <summary>
        /// Represents a randomly generated 2D directional vector with normalized length.
        /// This property utilizes noise-based randomness to produce a direction that is
        /// evenly distributed within the unit circle in a 2D space.
        /// </summary>
        public float2 Direction2D { get; }

        /// <summary>
        /// Generates a random 2-dimensional vector with each component constrained within specified ranges.
        /// </summary>
        /// <param name="minX">The minimum value for the X component of the generated vector. Default is <see cref="float.MinValue"/>.</param>
        /// <param name="maxX">The maximum value for the X component of the generated vector. Default is <see cref="float.MaxValue"/>.</param>
        /// <param name="minY">The minimum value for the Y component of the generated vector. Default is <see cref="float.MinValue"/>.</param>
        /// <param name="maxY">The maximum value for the Y component of the generated vector. Default is <see cref="float.MaxValue"/>.</param>
        /// <returns>A <see cref="float2"/> representing a random 2D vector within the specified ranges.</returns>
        public float2 Float2(float minX = float.MinValue, float maxX = float.MaxValue, float minY = float.MinValue,
            float maxY = float.MaxValue);

        /// <summary>
        /// Provides a random three-dimensional unit vector (float3) based on the current state of the random number generator.
        /// This allows the generation of a direction in 3D space uniformly distributed over the unit sphere.
        /// </summary>
        public float3 Direction3D  { get; }

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
        public float3 Float3(float minX = float.MinValue, float maxX = float.MaxValue, float minY = float.MinValue,
            float maxY = float.MaxValue, float minZ = float.MinValue, float maxZ = float.MaxValue);

        /// <summary>
        /// Represents a randomly generated 4-dimensional directional vector.
        /// Provides a normalized float4 value, ensuring each direction is uniformly distributed
        /// across the 4D space.
        /// </summary>
        public float4 Direction4D  { get; }

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
        public float4 Float4(float minX = float.MinValue, float maxX = float.MaxValue, float minY = float.MinValue,
            float maxY = float.MaxValue, float minZ = float.MinValue, float maxZ = float.MaxValue,
            float minW = float.MinValue, float maxW = float.MaxValue);

        /// <summary>
        /// Represents a boolean value generated based on a random function.
        /// Utilizes an underlying random noise function to produce true or false
        /// with an equal probability of approximately 50%.
        /// </summary>
        public bool Bool { get; }

        /// <summary>
        /// Determines whether a chance-based event occurs based on the given probability.
        /// </summary>
        /// <param name="probabilityOfTrue">A floating-point value between 0.0 and 1.0 representing the probability of returning true.</param>
        /// <returns>True if the event occurs based on the provided probability, otherwise false.</returns>
        public bool Chance(float probabilityOfTrue);

        #endregion

        #region Perlin

        /// <summary>
        /// Generates a Perlin noise value based on the provided 2D coordinates, seed, and noise type.
        /// The noise value is computed by hashing the corners of the surrounding grid and applying interpolation.
        /// </summary>
        /// <param name="x">
        /// The X coordinate (continuous) used for generating the noise value.
        /// </param>
        /// <param name="y">
        /// The Y coordinate (continuous) used for generating the noise value.
        /// </param>
        public float Perlin(float x, float y);

        /// <summary>
        /// Generates a Perlin noise value based on the input coordinates and a specified seed.
        /// This method calculates smooth noise by blending integer-based deterministic noise
        /// values at each corner of the cube enclosing the input coordinates.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the noise input.
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the noise input.
        /// </param>
        /// <param name="z">
        /// The z-coordinate of the noise input.
        /// </param>
        /// <returns>
        /// A single-precision floating-point value representing the Perlin noise at the given coordinates.
        /// </returns>
        public float Perlin(float x, float y, float z);

        #endregion
    }
}