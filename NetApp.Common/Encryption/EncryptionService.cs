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
        private readonly string _key;

        public EncryptionService(string key)
        {
            if (!string.IsNullOrWhiteSpace(key) && key.Length != 32)
            {
                throw new ArgumentException("Key must be 32 characters long.");
            }
            _key = key;
        }

        public string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            if (string.IsNullOrWhiteSpace(_key))
                return value;
            return AESDecrypt(value, _key);
        }

        public string Encrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            if (string.IsNullOrWhiteSpace(_key))
                return value;
            return AESEncrypt(value, _key);
        }
        private string AESEncrypt(string plainText, string key)
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
        private string AESDecrypt(string cipherText, string key)
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
