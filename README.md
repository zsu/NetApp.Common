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
     services.AddEncryptionService(options => {
         options.Key=Configuration.GetValue<string>("Encryption:Key");
         options.Iv = Configuration.GetValue<string>("Encryption:Iv");
     });
     services.AddCacheManager(options => {
         var redisConnectionstring = GetConnectionString("RedisConnection"); 
         options.RedisCacheOptions.Configuration = redisConnectionstring;
         options.RedisCacheOptions.InstanceName= $"{_env.EnvironmentName}/{Configuration.GetValue<string>("AppSettings:Application:Name")}/";
     });
     services.AddEmailService(options => {
         Configuration.GetSection("Email").Bind(options);
     });
     services.AddPasswordGenerator();
     services.AddLdapService(options => {
         Configuration.GetSection("Ldap").Bind(options);
     });
  ```
 * appsettings.json
 ```xml
   //AES Key and Iv
    "Encryption": {
     "Key": "xxxxx",
     "Iv": "xxx"
   },
   "Email": {
     "Smtp": "",
     "Port": 25,
     "UseSsl": false,
     "Sender": "WebFramework",
     "SenderEmail": "noreply@webframework",
     "UserName": "",
     "Password": "",
     //"TestEmail":"xxx@xxx"
   },
   "ConnectionStrings": {
     "RedisConnection":xxx
   }
  ```
# License
All source code is licensed under MIT license - http://www.opensource.org/licenses/mit-license.php
