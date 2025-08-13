using System.Threading.Tasks;
using UnityEngine;

namespace CoreFramework
{
    /// <summary>
    /// Provides extension methods for Unity's <see cref="AsyncOperation"/> to integrate with asynchronous programming patterns.
    /// </summary>
    public static class AsyncOperationExtensions
    {
        /// <summary>
        /// Converts a Unity <see cref="AsyncOperation"/> to a <see cref="Task"/> that can be awaited.
        /// </summary>
        /// <param name="asyncOperation">The <see cref="AsyncOperation"/> to convert.</param>
        /// <returns>A <see cref="Task"/> that represents the completion of the <see cref="AsyncOperation"/>.</returns>
        public static Task AsTask(this AsyncOperation asyncOperation)
        {
            var tcs = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}