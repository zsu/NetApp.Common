using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetApp.Common.Cache
{
    /// <summary>
    /// Extensions of ICacheManager
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Get default cache time in minutes
        /// </summary>
        private static int DefaultCacheTimeMinutes { get { return 60; } }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            //use default cache time
            return cacheManager.Get(key, acquire, DefaultCacheTimeMinutes);
        }
        public static T Get<T>(this ICacheManager cacheManager, string key)
        {
            //use default cache time
            return cacheManager.Get(key, () => { return default(T); }, DefaultCacheTimeMinutes);
        }
        public static void Set(this ICacheManager cacheManager, string key, object data)
        {
            cacheManager.SetAsync(key, data, DefaultCacheTimeMinutes).Wait();
        }
        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="pattern">Pattern</param>
        /// <param name="keys">All keys in the cache</param>
        public static void RemoveByPattern(this ICacheManager cacheManager, string pattern, IEnumerable<string> keys)
        {
            //get cache keys that matches pattern
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = keys.Where(key => regex.IsMatch(key)).ToList();

            //remove matching values
            //matchesKeys.ForEach(cacheManager.RemoveAsync);
            foreach (var item in matchesKeys)
                cacheManager.RemoveAsync(item).Wait();
        }
        public static void Clear(this ICacheManager cacheManager)
        {
            cacheManager.ClearAsync().Wait();
        }
        public static void Remove(this ICacheManager cacheManager, string key)
        {
            cacheManager.RemoveAsync(key).Wait();
        }
        public static void RemoveByPrefix(this ICacheManager cacheManager, string key)
        {
            cacheManager.RemoveByPrefixAsync(key).Wait();
        }
    }
}
