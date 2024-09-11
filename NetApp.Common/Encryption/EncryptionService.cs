using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetApp.Common
{
    public class EncryptionService : IEncryptionService
    {
        private EncryptionOptions _options;
        public EncryptionService(IOptions<EncryptionOptions> options):this(options?.Value)
        {}
        public EncryptionService(EncryptionOptions options)
        {
            _options = options;
        }
        public string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            var key = _options?.Key;
            var iv = _options?.Iv;
            if (string.IsNullOrWhiteSpace(key))
                return value;
            else
                if (string.IsNullOrWhiteSpace(iv))
                return EncryptProvider.AESDecrypt(value, key);
            return EncryptProvider.AESDecrypt(value, key, iv);
        }
        public string Encrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            var key = _options?.Key;
            var iv = _options?.Iv;
            if (string.IsNullOrWhiteSpace(key))
                return value;
            else
                if (string.IsNullOrWhiteSpace(iv))
                return EncryptProvider.AESEncrypt(value, key);
            return EncryptProvider.AESEncrypt(value, key, iv);
        }
    }
}
