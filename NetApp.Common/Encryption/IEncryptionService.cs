namespace NetApp.Common
{
    public interface IEncryptionService
    {
        string Decrypt(string value);
        string Encrypt(string value);
    }
}