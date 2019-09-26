using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="IEnumerable{T}"/> object.
    /// </summary>
    public static class EnumerableExtensions
    {
        //public static Task ForEachAsync<T>(this IEnumerable<T> sequence, Func<T, Task> action)
        //{
        //    return Task.WhenAll(sequence.Select(action));
        //}

        //public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        //{
        //    return Task.WhenAll(
        //        from partition in Partitioner.Create(source).GetPartitions(dop)
        //        select Task.Run(async delegate {
        //            using (partition)
        //                while (partition.MoveNext())
        //                    await body(partition.Current);
        //        }));
        //}

        //public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body)
        //{
        //    return Task.WhenAll(
        //        from item in source
        //        select Task.Run(() => body(item)));
        //}
    }
}