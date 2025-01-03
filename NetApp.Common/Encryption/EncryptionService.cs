using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NETCore.Encrypt;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NetApp.Common
{
    public class EncryptionService : IEncryptionService
    {
        private EncryptionOptions _options;
        public EncryptionService(IOptions<EncryptionOptions> options) : this(options?.Value)
        { }
        public EncryptionService(EncryptionOptions options)
        {
            _options = options;
            if (!string.IsNullOrWhiteSpace(_options?.Key) && (_options?.Key?.Length != 32))
            {
                throw new ArgumentException("Key must be 32 characters long.");
            }
        }
        public string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            var key = _options?.Key;
            if (string.IsNullOrWhiteSpace(key))
                return value;
            return AESDecrypt(key, value);
        }
        public string Encrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            var key = _options?.Key;
            if (string.IsNullOrWhiteSpace(key))
                return value;
            return AESEncrypt(key, value);
        }
        private string AESEncrypt(string key, string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        private string AESDecrypt(string key, string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return cipherText;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    byte[] iv = new byte[16];
                    ms.Read(iv, 0, iv.Length);
                    aes.IV = iv;
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }

        }
    }
}
