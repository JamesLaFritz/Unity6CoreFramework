using System;
using System.Collections.Generic;

namespace CoreFramework
{
    /// <summary>
    /// Provides extension methods for working with enumerable sequences.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs an action on each element in the sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence to iterate over.</param>
        /// <param name="action">The action to perform on each element.</param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
            {
                action(item);
            }
        }


        /// <summary>
        /// Returns a random element from the sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence to select the random element from.</param>
        /// <returns>A random element from the sequence.</returns>
        public static T Random<T>(this IEnumerable<T> sequence)
        {
            switch (sequence)
            {
                case null:
                    throw new ArgumentNullException(nameof(sequence));
                case IList<T> { Count: 0 } list:
                    throw new InvalidOperationException("Cannot get a random element from an empty collection.");
                case IList<T> list:
                    // ToDo: Convert to Random Noise
                    return list[UnityEngine.Random.Range(0, list.Count)];
            }

            // Use reservoir sampling when the input is not an IList<T> i.e.: a stream or lazy sequence
            using var enumerator = sequence.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Cannot get a random element from an empty collection.");

            var result = enumerator.Current;
            var count = 1;
            while (enumerator.MoveNext())
            {
                // ToDo: Convert to Random Noise
                if (UnityEngine.Random.Range(0, ++count) == 0)
                {
                    result = enumerator.Current;
                }
            }

            return result;
        }
    }
}