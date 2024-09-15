using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NETCore.Encrypt;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
            else if (string.IsNullOrWhiteSpace(iv))
            {
                if(value.Length < 16)
                    throw new Exception("Invalid encrypted value.");
                iv = value.Substring(0,16);
                var encryptedValue = value.Substring(16);
                return EncryptProvider.AESDecrypt(encryptedValue, key,iv);
            }
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
                {
                    iv = EncryptProvider.CreateAesKey().IV;
                }
            var encrypted=EncryptProvider.AESEncrypt(value, key, iv);
            if (string.IsNullOrWhiteSpace(_options?.Iv))
            {
                return $"{iv}{encrypted}";
            } 
            return encrypted;
        }
    }
}
