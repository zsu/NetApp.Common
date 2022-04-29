using System;
using System.Threading.Tasks;

namespace NetApp.Common.Cache
{
    /// <summary>
    /// Represents a null cache (caches nothing)
    /// </summary>
    public partial class NullCache : ICacheManager
    {
        public Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public T Get<T>(string key, Func<T> acquire, int? cacheTime)
        {
            return default(T);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime)
        {
            return Task.FromResult<T>(default(T));
        }

        public Task<T> GetAsync<T>(string key, Func<T> acquire, int? cacheTime)
        {
            return Task.FromResult<T>(default(T));
        }

        public Task RemoveAsync(string cacheKey)
        {
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            return Task.CompletedTask;
        }

        public Task SetAsync(string key, object data, int? cacheTime)
        {
            return Task.CompletedTask;
        }
    }
}