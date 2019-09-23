using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Caches objects from web requests.
    /// </summary>
    public class CacheManager : DisposableObject, ICacheManager
    {
        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager"/> class.
        /// </summary>
        /// <param name="logger">Logger for application caching.</param>
        /// <param name="cache">Memory cache for application caching.</param>
        public CacheManager(ILogger<CacheManager> logger, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keys = new ConcurrentDictionary<string, bool>();
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Dispose owned objects
            _cache?.Dispose();
        }

        #endregion

        #endregion

        /// <summary>
        /// The <see cref="ILogger"/> used for application logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="IMemoryCache"/> used for application caching.
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// <see cref="ConcurrentDictionary{TKey, TValue}"/> for key storage.
        /// </summary>
        private readonly ConcurrentDictionary<string, bool> _keys;

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> containing the keys of all cached elements.
        /// </summary>
        public IEnumerable<string> Keys
            => _keys.Where(kvp => kvp.Value).Select(kvp => kvp.Key);

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Resource type to return.</typeparam>
        /// <param name="key">The key identifying the entry to get.</param>
        /// <param name="value">Contains the object associated with the specified key.</param>
        public bool TryGetValue<T>(string key, out T value)
            => _cache.TryGetValue(key, out value);

        /// <summary>
        /// Gets element from cache or create if it not found.
        /// </summary>
        /// <typeparam name="T">Resource type to return.</typeparam>
        /// <param name="key">The key identifying the entry.</param>
        /// <param name="factory">Inline factory function used to create the object.</param>
        public async Task<T> GetOrCreateAsync<T>(object key, Func<ICacheEntry, Task<T>> factory)
        {
            if (key == null)
                throw new ArgumentException(nameof(key));

            var cacheKey = $"|{key.GetType().Name}|{key.GetHashCode()}";

            try
            {
                var cachedItem = await _cache.GetOrCreateAsync(cacheKey, factory);
                if (cachedItem != null)
                {
                    _keys.TryAdd(cacheKey, true);
                    _logger.LogDebug($"The element with key '{cacheKey}' was read from cache.");
                    return cachedItem;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The element with key '{key}' failed to read from cache.");
            }



            return default;
        }

        /// <summary>
        /// Removes the cache element associated with the specified key.
        /// </summary>
        /// <param name="key">A key identifying the entry.</param>
        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(nameof(key));

            _cache.Remove(key);
            _keys.TryRemove(key, out bool value);
        }

        /// <summary>
        /// Removes all elements from cache with the specified key prefix.
        /// </summary>
        /// <param name="prefix">A key prefix identifying the entry.</param>
        public void Clear(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException(nameof(prefix));

            if (_keys.Count > 0)
            {
                var keys = _keys.Where(kvp => kvp.Key.StartsWith(prefix.Trim(), StringComparison.OrdinalIgnoreCase));
                foreach (var key in keys)
                {
                    _cache.Remove(key);
                    _keys.TryRemove(key.Key, out bool value);
                }
            }
        }


        /// <summary>
        /// Removes all elements from cache with the specified key.
        /// </summary>
        /// <param name="key">The key identifying the entry to clear.</param>
        public void Clear(CacheKeys key)
        {
            if (key == null)
                throw new ArgumentException(nameof(key));

            var cacheKey = $"|{key}|";

            Clear(cacheKey);
        }

        /// <summary>
        /// Removes all elements from cache.
        /// </summary>
        public void Clear()
        {
            foreach (var key in _keys)
            {
                _cache.Remove(key);
            }
            _keys.Clear();
        }
    }
}
