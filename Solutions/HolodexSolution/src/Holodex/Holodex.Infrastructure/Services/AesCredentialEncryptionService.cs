using System.Security.Cryptography;
using System.Text;
using Holodex.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Holodex.Infrastructure.Services;

public class AesCredentialEncryptionService : ICredentialEncryptionService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private readonly byte[] _key;

    public AesCredentialEncryptionService(IConfiguration config)
    {
        var keyBase64 = config["Credentials:EncryptionKey"]
            ?? throw new InvalidOperationException("Credentials:EncryptionKey is not configured.");
        _key = Convert.FromBase64String(keyBase64);
    }

    public string Encrypt(string plaintext)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var ciphertext = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plainBytes, ciphertext, tag);

        var result = new byte[NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, NonceSize + ciphertext.Length, TagSize);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string ciphertext)
    {
        var data = Convert.FromBase64String(ciphertext);
        var nonce = data.AsSpan(0, NonceSize);
        var cipherBytes = data.AsSpan(NonceSize, data.Length - NonceSize - TagSize);
        var tag = data.AsSpan(data.Length - TagSize, TagSize);

        var plainBytes = new byte[cipherBytes.Length];
        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
