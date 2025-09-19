using System.Collections.Generic;

namespace CoreFramework
{
    /// <summary>
    /// Provides extension methods for enumerator manipulation.
    /// </summary>
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// Converts an IEnumerator to an IEnumerable.
        /// </summary>
        /// <param name="e">An instance of IEnumerator.</param>
        /// <returns>An IEnumerable with the same elements as the input instance.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }
}