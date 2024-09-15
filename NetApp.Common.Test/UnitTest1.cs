using NuGet.Frameworks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace NetApp.Common.Test
{
    [TestClass]
    public class UnitTest1
    {
        private EncryptionOptions _options;
        private IEncryptionService _encryptionService;
        [TestInitialize]
        public void Setup()
        {
            _options = new EncryptionOptions { Key = "6EC874CCB8B8E24E9F912C4381A0C2FA", Iv = "g0W90jtQ0DxEveXi" };
            _encryptionService = new EncryptionService(_options);
        }
        [TestMethod]
        public void SpecialCharacters()
        {
            var value1 = "abcd\\, *, +, ?, |, {, [, (,), ^, $, ., #,&,&amp;";
            var value2 = @"abcd\, *, +, ?, |, {, [, (,), ^, $, ., #,&,&amp;";
            var encryptedValue = _encryptionService.Encrypt(value1);
            var decryptedVallue = _encryptionService.Decrypt(encryptedValue);
            Assert.IsTrue(encryptedValue != value1);
            Assert.IsTrue(decryptedVallue == value1);
            encryptedValue = _encryptionService.Encrypt(value2);
            decryptedVallue = _encryptionService.Decrypt(encryptedValue);
            Assert.IsTrue(encryptedValue != value2);
            Assert.IsTrue(decryptedVallue == value2);
        }

        [TestMethod]
        public void EncryptAndDecryptWithoutIv()
        {
            var options = new EncryptionOptions { Key = "6EC874CCB8B8E24E9F912C4381A0C2FA" };
            var encryptionService = new EncryptionService(options);

            var value = "Hello, World!";
            var encryptedValue = encryptionService.Encrypt(value);
            var decryptedValue = encryptionService.Decrypt(encryptedValue);
            Assert.IsTrue(decryptedValue == value);
        }
    }
}