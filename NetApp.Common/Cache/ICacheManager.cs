using System;
using System.Threading.Tasks;

namespace NetApp.Common.Cache
{
    public interface ICacheManager : IDisposable
    {
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime);

        Task<T> GetAsync<T>(string key, Func<T> acquire, int? cacheTime);

        T Get<T>(string key, Func<T> acquire, int? cacheTime);

        Task RemoveAsync(string cacheKey);

        Task SetAsync(string key, object data, int? cacheTime);

        Task RemoveByPrefixAsync(string prefix);
        Task ClearAsync();
    }
    public interface ICacheManager<T> : ICacheManager { }

}
