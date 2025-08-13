using Unity.Mathematics;
using UnityEngine;

namespace CoreFramework
{
    /// <summary>
    /// Provides extension methods for the <see cref="int"/> and <see cref="float"/> types.
    /// </summary>
    public static class NumberExtensions
    {
        public static float PercentageOf(this int part, int whole)
        {
            if (whole == 0) return 0; // Handling division by zero
            return (float)part / whole;
        }

        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
        public static bool IsOdd(this int i) => i % 2 == 1;
        public static bool IsEven(this int i) => i % 2 == 0;

        public static int AtLeast(this int value, int min) => math.max(value, min);
        public static int AtMost(this int value, int max) => math.min(value, max);
        
        public static half AtLeast(this half value, half max) => (half)math.max(value, max);
        public static half AtMost(this half value, half max) => (half)math.min(value, max);

        public static float AtLeast(this float value, float min) => math.max(value, min);
        public static float AtMost(this float value, float max) => math.min(value, max);

        public static double AtLeast(this double value, double min) => math.max(value, min);
        public static double AtMost(this double value, double min) => math.min(value, min);
    }
}