using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using NativeApp.Core;
using NativeApp.Core.Models;

namespace NativeApp.Windows;

/// <summary>Persists tokens DPAPI-encrypted (current-user scope) under %LOCALAPPDATA% - readable
/// only by this Windows user account, no separate secret/password needed to protect it.</summary>
public class DpapiTokenStore : ITokenStore
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Brian", "NativeApp", "tokens.dat");

    public AuthTokens? Load()
    {
        if (!File.Exists(FilePath))
            return null;

        try
        {
            var encrypted = File.ReadAllBytes(FilePath);
            var decrypted = ProtectedData.Unprotect(encrypted, optionalEntropy: null, DataProtectionScope.CurrentUser);
            return JsonSerializer.Deserialize<AuthTokens>(decrypted);
        }
        catch
        {
            // Corrupted, from a different user profile, or DPAPI key unavailable - treat as "not logged in".
            return null;
        }
    }

    public void Save(AuthTokens tokens)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var plain = JsonSerializer.SerializeToUtf8Bytes(tokens);
        var encrypted = ProtectedData.Protect(plain, optionalEntropy: null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(FilePath, encrypted);
    }

    public void Clear()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }
}
