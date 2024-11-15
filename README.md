[![NuGet](https://img.shields.io/nuget/v/NetApp.Common.svg)](https://www.nuget.org/packages/NetApp.Common)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

# What is NetApp.Common

Cache, Encryption, Email, Password Generator, Util libraries for .Net

# NuGet
```xml
Install-Package NetApp.Common
```
# Getting started with NetApp.Common

  * Call the followings in Startup:  
  ```xml
     services.AddEncryptionService(options => {
         options.Key=Configuration.GetValue<string>("Encryption:Key"); //32 bytes key
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
     /*"TestEmail":"xxx@xxx",
     "OAuth2":{
         GrantType: "client_credentials",
         ClientId: "xxx",
         ClientSecret: "xxx",
         TenantId: "xxx",
         TokenUri: "https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token",
         Scopes: "xxx,xxx"
     }*/
   },
   "ConnectionStrings": {
     "RedisConnection":xxx
   }
  ```
# License
All source code is licensed under MIT license - http://www.opensource.org/licenses/mit-license.php
