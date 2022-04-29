using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace NetApp.Common.Cache
{
    public class PerRequestCacheManager
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ReaderWriterLockSlim _lockSlim;

        #endregion

        #region Ctor

        public PerRequestCacheManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _lockSlim = new ReaderWriterLockSlim();
        }

        #endregion

        #region Utilities

        protected virtual IDictionary<object, object> GetItems()
        {
            return _httpContextAccessor.HttpContext?.Items;
        }

        #endregion

        #region Methods
        public virtual T Get<T>(string key, Func<T> acquire)
        {
            IDictionary<object, object> items;

            _lockSlim.EnterReadLock();
            try
            {
                items = GetItems();
                if (items == null)
                    return acquire();

                if (items[key] != null)
                    return (T)items[key];
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }

            var result = acquire();

            _lockSlim.EnterWriteLock();
            try
            {
                items[key] = result;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
            return result;
        }

        public virtual void Set(string key, object data)
        {
            if (data == null)
                return;

            _lockSlim.EnterWriteLock();
            try
            {
                var items = GetItems();
                if (items == null)
                    return;

                items[key] = data;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public virtual bool IsSet(string key)
        {
            _lockSlim.EnterReadLock();
            try
            {
                var items = GetItems();
                return items?[key] != null;
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }
        public virtual void Remove(string key)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                var items = GetItems();
                items?.Remove(key);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public virtual void RemoveByPrefix(string prefix)
        {
            _lockSlim.EnterUpgradeableReadLock();
            try
            {
                var items = GetItems();
                if (items == null)
                    return;

                var regex = new Regex(prefix,
                    RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matchesKeys = items.Keys.Select(p => p.ToString())
                    .Where(key => regex.IsMatch(key ?? string.Empty)).ToList();

                if (!matchesKeys.Any())
                    return;

                _lockSlim.EnterWriteLock();
                try
                {
                    foreach (var key in matchesKeys)
                        items.Remove(key);
                }
                finally
                {
                    _lockSlim.ExitWriteLock();
                }
            }
            finally
            {
                _lockSlim.ExitUpgradeableReadLock();
            }
        }

        #endregion
    }
}
