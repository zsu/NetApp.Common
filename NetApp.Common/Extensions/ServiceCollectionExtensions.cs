using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetApp.Common.Cache;
using NetApp.Common.Security;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetApp.Common
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEncryptionService(this IServiceCollection services, Action<string> setupAction)
        {
            services.AddOptions<string>().Configure(setupAction);
            services.AddTransient<IEncryptionService, EncryptionService>();
            return services;
        }
        public static IServiceCollection AddEmailService(this IServiceCollection services, Action<EmailSettings> setupAction)
        {
            services.AddOptions<EmailSettings>().Configure(setupAction);
            services.AddTransient<IEmailService, EmailService>();
            return services;
        }
        public static IServiceCollection AddPasswordGenerator(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
            return services;
        }
        public static IServiceCollection AddCacheManager(this IServiceCollection services, Action<CacheManagerOptions> setupAction)
        {
            services.AddOptions<CacheManagerOptions>().Configure(setupAction);
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddSingleton<ICacheManager<MemoryCacheManager>, MemoryCacheManager>();
            services.AddSingleton<ICacheManager<DistributedCacheManager>>(provider =>
            {
                var cacheOptions = provider.GetRequiredService<IOptions<CacheManagerOptions>>();
                services.AddStackExchangeRedisCache(options => {
                    options = cacheOptions?.Value.RedisCacheOptions;
                });
                return new DistributedCacheManager(provider.GetRequiredService<IDistributedCache>());
            });
            return services;
        }
    }
}
