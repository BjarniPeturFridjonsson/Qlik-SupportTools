using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eir.Common.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits all of the tasks in the given sequence. If any of the tasks fails, an exception is thrown containing
        /// exception information from all failed tasks.
        /// </summary>
        public static async Task AwaitAll(this IEnumerable<Task> tasks)
        {
            var taskArray = tasks as Task[] ?? tasks.ToArray();
            try
            {
                await Task.WhenAll(taskArray).ConfigureAwait(false);
            }
            catch (Exception)
            {
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                tcs.TrySetException(new AggregateException(taskArray.Where(t => t.IsFaulted).Select(t => t.Exception.Flatten())));
                await tcs.Task;
            }
        }

        /// <summary>
        /// Awaits all of the tasks in the given sequence. If any of the tasks fails, an exception is thrown containing
        /// exception information from all failed tasks.
        /// </summary>
        public static async Task<IEnumerable<T>> AwaitAll<T>(this IEnumerable<Task<T>> tasks)
        {
            T[] result = { };
            var taskArray = tasks as Task<T>[] ?? tasks.ToArray();
            try
            {
                result = await Task.WhenAll(taskArray);
            }
            catch (Exception)
            {
                var tcs = new TaskCompletionSource<T>();
                tcs.TrySetException(new AggregateException(taskArray.Where(t => t.IsFaulted).Select(t => t.Exception.Flatten())));
                await tcs.Task;
            }

            return result;
        }

        public static void SafeWait(this Task task)
        {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static T SafeWait<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}