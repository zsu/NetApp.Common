using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace NetApp.Common.Cache
{
    public partial class DistributedCacheManager : ICacheManager, ICacheManager<DistributedCacheManager>
    {
        #region Fields

        private readonly IDistributedCache _distributedCache;
        //private readonly PerRequestCache _perRequestCache;
        private static readonly List<string> _keys;
        private static readonly AsyncLock _locker;

        #endregion

        #region Ctor

        static DistributedCacheManager()
        {
            _locker = new AsyncLock();
            _keys = new List<string>();
        }
        public DistributedCacheManager(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public DistributedCacheManager(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            //_perRequestCache = new PerRequestCache(httpContextAccessor);
        }

        #endregion

        #region Utilities

        private DistributedCacheEntryOptions PrepareEntryOptions(int cacheTime)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime)
            };

            return options;
        }

        private async Task<(bool isSet, T item)> TryGetItemAsync<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);

            if (string.IsNullOrEmpty(json))
                return (false, default);

            var item = JsonConvert.DeserializeObject<T>(json);
            //_perRequestCache.Set(key, item);

            return (true, item);
        }

        private (bool isSet, T item) TryGetItem<T>(string key)
        {
            var json = _distributedCache.GetString(key);

            if (string.IsNullOrEmpty(json))
                return (false, default);

            var item = JsonConvert.DeserializeObject<T>(json);
            //_perRequestCache.Set(key, item);

            return (true, item);
        }

        private void Set(string key, object data, int? cacheTime)
        {
            if ((cacheTime ?? 0) <= 0 || data == null)
                return;

            _distributedCache.SetString(key, JsonConvert.SerializeObject(data), PrepareEntryOptions(cacheTime.Value));
            //_perRequestCache.Set(key, data);

            using var _ = _locker.Lock();
            _keys.Add(key);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime)
        {
            //if (_perRequestCache.IsSet(key))
            //    return _perRequestCache.Get(key, () => default(T));

            if (cacheTime <= 0)
                return await acquire();

            var (isSet, item) = await TryGetItemAsync<T>(key);

            if (isSet)
                return item;

            var result = await acquire();

            if (result != null)
                await SetAsync(key, result, cacheTime);

            return result;
        }

        public async Task<T> GetAsync<T>(string key, Func<T> acquire, int? cacheTime)
        {
            //if (_perRequestCache.IsSet(key))
            //    return _perRequestCache.Get(key, () => default(T));

            if (cacheTime <= 0)
                return acquire();

            var (isSet, item) = await TryGetItemAsync<T>(key);

            if (isSet)
                return item;

            var result = acquire();

            if (result != null)
                await SetAsync(key, result, cacheTime);

            return result;
        }

        public T Get<T>(string key, Func<T> acquire, int? cacheTime)
        {
            //if (_perRequestCache.IsSet(key))
            //    return _perRequestCache.Get(key, () => default(T));

            if ((cacheTime ?? 0) <= 0)
                return acquire();

            var (isSet, item) = TryGetItem<T>(key);

            if (isSet)
                return item;

            var result = acquire();

            if (result != null)
                Set(key, result, cacheTime);

            return result;
        }
        public async Task RemoveAsync(string cacheKey)
        {
            await _distributedCache.RemoveAsync(cacheKey);
            //_perRequestCache.Remove(cacheKey);

            using var _ = await _locker.LockAsync();
            _keys.Remove(cacheKey);
        }
        public async Task SetAsync(string key, object data, int? cacheTime)
        {
            if ((cacheTime ?? 0) <= 0 || data == null)
                return;

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(data), PrepareEntryOptions(cacheTime.Value));
            //_perRequestCache.Set(key, data);

            using var _ = await _locker.LockAsync();
            _keys.Add(key);
        }


        public async Task RemoveByPrefixAsync(string prefix)
        {
            //_perRequestCache.RemoveByPattern(prefix);

            using var _ = await _locker.LockAsync();

            foreach (var key in _keys.Where(key => key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList())
            {
                await _distributedCache.RemoveAsync(key);
                _keys.Remove(key);
            }
        }

        public async Task ClearAsync()
        {
            //foreach (var redisKey in _keys)
            //    _perRequestCache.Remove(redisKey);

            using var _ = await _locker.LockAsync();

            foreach (var key in _keys)
                await _distributedCache.RemoveAsync(key);

            _keys.Clear();
        }
        public bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action)
        {
            if (!string.IsNullOrEmpty(_distributedCache.GetString(resource)))
                return false;

            try
            {
                _distributedCache.SetString(resource, resource, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime
                });

                action();

                return true;
            }
            finally
            {
                _distributedCache.Remove(resource);
            }
        }

        #endregion

    }
}