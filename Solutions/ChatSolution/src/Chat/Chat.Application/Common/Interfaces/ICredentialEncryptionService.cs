namespace Chat.Application.Common.Interfaces;

public interface ICredentialEncryptionService
{
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
}
