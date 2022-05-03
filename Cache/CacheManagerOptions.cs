using Microsoft.Extensions.Caching.StackExchangeRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetApp.Common
{
    public sealed class CacheManagerOptions
    {
        public RedisCacheOptions RedisCacheOptions { get; set; }
    }
}
