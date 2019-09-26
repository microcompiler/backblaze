using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// An interface for <see cref="ICacheManager"/>.
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> containing the keys of all cached elements.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Resource type to return.</typeparam>
        /// <param name="key">The key identifying the entryto get.</param>
        /// <param name="value">Contains the object associated with the specified key.</param>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Gets element from cache or create if it not found.
        /// </summary>
        /// <typeparam name="TItem">Resource type to return.</typeparam>
        /// <param name="key">A key identifying the entry.</param>
        /// <param name="factory">Inline factory function used to create the object.</param>
        Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory);

        /// <summary>
        /// Removes the cache element associated with the specified key.
        /// </summary>
        /// <param name="key">A key identifying the entry.</param>
        void Remove(string key);

        /// <summary>
        /// Removes all elements from cache with the specified key prefix.
        /// </summary>
        /// <param name="prefix">A key prefix identifying the entry.</param>
        void Clear(string prefix);

        /// <summary>
        /// Removes all elements from cache with the specified key.
        /// </summary>
        /// <param name="key">The key identifying the entry to clear.</param>
        void Clear(CacheKeys key);

        /// <summary>
        /// Removes all elements from cache.
        /// </summary>
        void Clear();
    }
}