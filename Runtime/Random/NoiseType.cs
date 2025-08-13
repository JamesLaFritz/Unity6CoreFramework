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

    public static class NoiseTypeExtensions
    {
        public static string ToDescriptionString(this NoiseType value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}