using System.ComponentModel;
using System.Reflection;

namespace CoreFramework.Random
{

    /// <summary>
    /// Specifies the type of noise algorithm used for generating pseudorandom noise values.
    /// </summary>
    public enum NoiseType
    {
        [Description("Mangled Bits")] MangledBits,

        [Description("Mangled Bits Balanced Mix")]
        MangledBitsBalancedMix,

        [Description("Mangled Bits Rotational")]
        MangledBitsRotational,

        [Description("ChaCha Quarter Round Simple")]
        ChaChaQuarterRoundSimple,

        [Description("ChaCha Quarter Round Compact Mixing 2")]
        ChaChaQuarterRoundAdvanced,

        [Description(
            "Not a vaild type this is for use in for loops, for (var typeIndex = 0; typeIndex < (int)NoiseType.All; typeIndex++)")]
        All
    }

    /// <summary>
    /// Provides extension methods for the <see cref="NoiseType"/> enumeration.
    /// </summary>
    public static class NoiseTypeExtensions
    {
        /// <summary>
        /// Converts the specified <see cref="NoiseType"/> enumeration value to its associated description string.
        /// </summary>
        /// <param name="value">The <see cref="NoiseType"/> value to convert.</param>
        /// <returns>The description string associated with the specified <see cref="NoiseType"/> value, or the enumeration name if no description is defined.</returns>
        public static string ToDescriptionString(this NoiseType value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}