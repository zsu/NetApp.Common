using System;
using System.Threading.Tasks;

namespace NetApp.Common.Cache
{
    public interface IAutoRefreshCache
    {
        T Get<T>(string key, Func<T> acquire, int? cacheTime = 60, bool autoRefresh = true);
        void Set<T>(string key, object data, Func<T> acquire, int? cacheTime = 60, bool autoRefresh = true);
    }

}
