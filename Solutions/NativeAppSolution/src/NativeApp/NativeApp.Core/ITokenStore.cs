using NativeApp.Core.Models;

namespace NativeApp.Core;

/// <summary>Persists auth tokens across app restarts. The Windows implementation uses DPAPI;
/// this abstraction is what would let a future non-Windows client swap in its own storage.</summary>
public interface ITokenStore
{
    AuthTokens? Load();
    void Save(AuthTokens tokens);
    void Clear();
}
