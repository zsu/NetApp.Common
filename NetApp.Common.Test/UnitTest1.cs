using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace NetApp.Common.Test
{
    [TestClass]
    public class UnitTest1
    {
        private string _key;
        private IEncryptionService _encryptionService;
        [TestInitialize]
        public void Setup()
        {
            _key =  "6EC874CCB8B8E24E9F912C4381A0C2FA" ;
            _encryptionService = new EncryptionService(_key);
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
        public void EncryptAndDecryptWithoutKey()
        {
            string key=null;
            var encryptionService = new EncryptionService(key);

            var value = "Hello, World!";
            var encryptedValue = encryptionService.Encrypt(value);
            var decryptedValue = encryptionService.Decrypt(encryptedValue);
            Assert.IsTrue(decryptedValue == value);
        }
    }
}