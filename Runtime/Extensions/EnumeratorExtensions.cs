using System.Collections.Generic;

namespace CoreFramework
{
    /// <summary>
    /// Provides extension methods for enumerator manipulation.
    /// </summary>
    public static class EnumeratorExtensions
    {
        // ReSharper disable InvalidXmlDocComment
        /// <summary>
        /// Converts an IEnumerator<T> to an IEnumerable<T>.
        /// </summary>
        /// <param name="e">An instance of IEnumerator<T>.</param>
        /// <returns>An IEnumerable<T> with the same elements as the input instance.</returns>
        // ReSharper restore InvalidXmlDocComment
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }
}