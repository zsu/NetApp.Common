using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace NetApp.Common.Cache
{
    public partial class MemoryCacheManager : ICacheManager, ICacheManager<MemoryCacheManager>, IFileDependencyCache, IAutoRefreshCache
    {
        #region Fields

        private bool _disposed;

        private readonly IMemoryCache _memoryCache;

        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixes = new ConcurrentDictionary<string, CancellationTokenSource>();
        private static CancellationTokenSource _clearToken = new CancellationTokenSource();

        #endregion

        #region Ctor

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #endregion

        #region Utilities

        private MemoryCacheEntryOptions PrepareEntryOptions(string key, int cacheTime)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime)
            };

            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));

            var tokenSource = _prefixes.GetOrAdd(key, new CancellationTokenSource());
            options.AddExpirationToken(new CancellationChangeToken(tokenSource.Token));

            return options;
        }

        private void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        private void Set(string key, object data, int? cacheTime)
        {
            if ((cacheTime ?? 0) <= 0 || data == null)
                return;

            _memoryCache.Set(key, data, PrepareEntryOptions(key, cacheTime.Value));
        }
        public void Set<T>(string key, object data, Func<T> acquire, int? cacheTime, bool autoRefresh)
        {
            if ((cacheTime ?? 0) <= 0 || data == null)
                return;
            if (autoRefresh)
                _memoryCache.Set(key, data, GetAutoRefreshOptions(acquire, cacheTime.Value));
            else
                _memoryCache.Set(key, data, PrepareEntryOptions(key, cacheTime.Value));
        }
        #endregion

        #region Methods

        public Task RemoveAsync(string cacheKey)
        {
            Remove(cacheKey);

            return Task.CompletedTask;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime)
        {
            if ((cacheTime ?? 0) <= 0)
                return await acquire();

            if (_memoryCache.TryGetValue(key, out T result))
                return result;

            result = await acquire();

            if (result != null)
                await SetAsync(key, result, cacheTime);

            return result;
        }

        public async Task<T> GetAsync<T>(string key, Func<T> acquire, int? cacheTime)
        {
            if ((cacheTime ?? 0) <= 0)
                return acquire();

            var result = _memoryCache.GetOrCreate(key, entry =>
            {
                entry.SetOptions(PrepareEntryOptions(key, cacheTime.Value));

                return acquire();
            });

            if (result == null)
                await RemoveAsync(key);

            return result;
        }

        public T Get<T>(string key, Func<T> acquire, int? cacheTime)
        {
            if ((cacheTime ?? 0) <= 0)
                return acquire();

            if (_memoryCache.TryGetValue(key, out T result))
                return result;

            result = acquire();

            if (result != null)
                Set(key, result, cacheTime);

            return result;
        }
        public T Get<T>(string key, Func<T> acquire, int? cacheTime, bool autoRefresh)
        {
            if ((cacheTime ?? 0) <= 0)
                return acquire();

            if (_memoryCache.TryGetValue(key, out T result))
                return result;

            result = acquire();

            if (result != null)
                Set(key, result, acquire, cacheTime, autoRefresh);

            return result;
        }
        public Task SetAsync(string key, object data, int? cacheTime)
        {
            Set(key, data, cacheTime);

            return Task.CompletedTask;
        }

        public bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
        {
            if (_memoryCache.TryGetValue(key, out _))
                return false;

            try
            {
                _memoryCache.Set(key, key, expirationTime);

                action();

                return true;
            }
            finally
            {
                _memoryCache.Remove(key);
            }
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            _prefixes.TryRemove(prefix, out var tokenSource);
            tokenSource?.Cancel();
            tokenSource?.Dispose();

            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _clearToken.Cancel();
            _clearToken.Dispose();

            _clearToken = new CancellationTokenSource();

            foreach (var prefix in _prefixes.Keys.ToList())
            {
                _prefixes.TryRemove(prefix, out var tokenSource);
                tokenSource?.Dispose();
            }

            return Task.CompletedTask;
        }
        public virtual void Set(string key, object value, FileCacheDependency dependency)
        {
            var fileInfo = new FileInfo(dependency.FileName);
            var fileProvider = new PhysicalFileProvider(fileInfo.DirectoryName);
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions().AddExpirationToken(fileProvider.Watch(fileInfo.Name)));

        }
        private MemoryCacheEntryOptions GetAutoRefreshOptions<T>(Func<T> acquire, int cacheTime)
        {
            var expirationTime = DateTime.Now.AddMinutes(cacheTime);
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(TimeSpan.FromSeconds(cacheTime * 60 + .01)).Token);

            var options = new MemoryCacheEntryOptions();
            options.SetAbsoluteExpiration(expirationTime);
            options.AddExpirationToken(expirationToken);

            options.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration()
            {
                EvictionCallback = (key, value, reason, state) =>
                {
                    if (reason == EvictionReason.TokenExpired || reason == EvictionReason.Expired)
                    {
                        var newValue = acquire();
                        if (newValue != null)
                        {
                            _memoryCache.Set(key, newValue, GetAutoRefreshOptions<T>(acquire, cacheTime));
                        }
                        else
                        {
                            _memoryCache.Set(key, value, GetAutoRefreshOptions<T>(acquire, cacheTime));
                        }
                    }
                }
            });

            return options;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _memoryCache.Dispose();

            _disposed = true;
        }

        #endregion
    }
}