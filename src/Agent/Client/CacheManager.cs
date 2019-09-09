using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

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
            _logger = logger ?? new LoggerFactory().CreateLogger<CacheManager>();
            _cache = cache ?? new MemoryCache(new MemoryCacheOptions());
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
        /// <see cref="ILogger"/> for application logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// <see cref="IMemoryCache"/> for application caching.
        /// </summary>
        private readonly IMemoryCache _cache;

        protected static readonly HashSet<string> CacheKeys = new HashSet<string>();

        public async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<ICacheEntry, Task<TItem>> factory)
        {
            CacheKeys.Add(key);
            _logger.LogInformation($"ADD {key}");
            return await _cache.GetOrCreateAsync(key, factory);
        }

        /// <summary>
        /// Removes the cache associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void RemoveByPattern(string pattern)
        {
            var keysToRemove = CacheKeys
                .Where(k => Regex.IsMatch(k, pattern, RegexOptions.IgnoreCase))
                .ToArray();
            foreach (var ktr in keysToRemove)
                Remove(ktr);
        }

        /// <summary>
        /// Removes the cache associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove(string key)
        {
            _cache.Remove(key);
            _logger.LogInformation($"Remove {key}");
            CacheKeys.Remove(key);
        }
    }
}
