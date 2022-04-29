using System;
using System.Threading.Tasks;

namespace NetApp.Common.Cache
{
    public interface IFileDependencyCache
    {
        void Set(string key, object value, FileCacheDependency dependency);
    }

}
