[![NuGet](https://img.shields.io/nuget/v/NetApp.Common.svg)](https://www.nuget.org/packages/NetApp.Common)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

# What is NetApp.Common

Cache, Encryption, Password Generator, Util libraries for .Net

# NuGet
```xml
Install-Package NetApp.Common
```
# Getting started with NetApp.Common

  * Call the followings in Startup:  
  ```xml
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddSingleton<ICacheManager<MemoryCacheManager>, MemoryCacheManager>();
            var redisConnectionstring = GetConnectionString("RedisConnection");
            if(!string.IsNullOrWhiteSpace(redisConnectionstring))
                services.AddStackExchangeRedisCache(options => {
                    options.Configuration = redisConnectionstring;
                    options.InstanceName = $"{_env.EnvironmentName}/{Configuration.GetValue<string>("AppSettings:Application:Name")}/";
                });
            services.AddSingleton<ICacheManager<DistributedCacheManager>, DistributedCacheManager>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
  ```

# License
All source code is licensed under MIT license - http://www.opensource.org/licenses/mit-license.php
