using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetApp.Common
{
    public class EncryptionService : IEncryptionService
    {
        private IConfiguration _configuration;
        public EncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");
            var key = _configuration.GetValue<string>("EncryptionKey:Key");
            var iv = _configuration.GetValue<string>("EncryptionKey:Iv");
            //var encyrptedString = EncryptProvider.AESEncrypt(Configuration.GetConnectionString(name), key, iv);
            if (string.IsNullOrWhiteSpace(key))
                return value;
            else
                if (string.IsNullOrWhiteSpace(iv))
                return Regex.Unescape(EncryptProvider.AESDecrypt(value, key));
            return Regex.Unescape(EncryptProvider.AESDecrypt(value, key, iv));
        }
        public string Encrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");
            var key = _configuration.GetValue<string>("EncryptionKey:Key");
            var iv = _configuration.GetValue<string>("EncryptionKey:Iv");
            //var encyrptedString = EncryptProvider.AESEncrypt(Configuration.GetConnectionString(name), key, iv);
            if (string.IsNullOrWhiteSpace(key))
                return value;
            else
                if (string.IsNullOrWhiteSpace(iv))
                return Regex.Unescape(EncryptProvider.AESEncrypt(value, key));
            return Regex.Unescape(EncryptProvider.AESEncrypt(value, key, iv));
        }
    }
}
