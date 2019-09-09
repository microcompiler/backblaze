using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Bytewizer.Backblaze.Client
{
    public interface ICacheManager
    {
        Task<TItem> GetOrCreateAsync<TItem>(string key, Func<ICacheEntry, Task<TItem>> factory);
        void Remove(string key);
        void RemoveByPattern(string pattern);
    }
}