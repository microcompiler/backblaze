using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Bytewizer.Backblaze.Models;
using System.Threading;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="IEnumerable{T}"/> object.
    /// </summary>
    public static class EnumerableExtensions
    {
        //public static async Task<IEnumerable<T>> AsEnumerableAsync<T>(this IEnumerable<T> source, int dop, Func<IApiResults<T>> action)
        //{
        //    var response = new List<T>();
        //    await source.ForEachAsync(dop, item =>
        //    {
        //        var results = action.Invoke();
        //        if (results.IsSuccessStatusCode)
        //        {
        //            response.Add(results.Response);
        //        }
        //    });
        //    return response;
        //}

        /// <summary>
        /// Runs the specified async method for each item of the input collection in a parallel manner.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="source">The collection of elements which can be enumerated asynchronously.</param>
        /// <param name="dop">The degree of parallelism. Use 0 to default to <see cref="Environment.ProcessorCount"/>.</param>
        /// <param name="action">A synchronous action to perform for every single item in the collection.</param>
        /// <param name="cancellationToken">A cancellation token to stop enumerating.</param>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> action, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            dop = (dop <= 0) ? Environment.ProcessorCount : dop;
            dop = (dop > source.Count()) ? dop: source.Count(); 

            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext() && !cancellationToken.IsCancellationRequested)
                        {
                            await action(partition.Current);
                        }
                }, cancellationToken));
        }
    }
}