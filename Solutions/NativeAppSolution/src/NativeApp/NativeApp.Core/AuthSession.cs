using System.Text.Json;
using NativeApp.Core.Models;

namespace NativeApp.Core;

/// <summary>
/// Owns the current access/refresh token pair for the app's lifetime: loads whatever was
/// persisted at startup, silently refreshes an expired access token, and is the single source
/// both ApiClient callers and NotificationClient's SignalR AccessTokenProvider read from - so a
/// mid-session refresh is visible to an already-open hub connection rather than a stale capture.
/// </summary>
public class AuthSession(ApiClient apiClient, ITokenStore tokenStore)
{
    private static readonly TimeSpan ExpiryBuffer = TimeSpan.FromSeconds(30);

    private AuthTokens? _tokens;

    public async Task LoginAsync(string email, string password, CancellationToken ct = default)
    {
        _tokens = await apiClient.LoginAsync(email, password, ct);
        tokenStore.Save(_tokens);
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var current = _tokens ?? tokenStore.Load();
        if (current is not null)
        {
            try { await apiClient.LogoutAsync(current.AccessToken, current.RefreshToken, ct); }
            catch { /* best effort - local state is cleared regardless */ }
        }

        _tokens = null;
        tokenStore.Clear();
    }

    /// <summary>Returns a currently-valid access token, refreshing first if needed, or null if the
    /// caller needs to show the login window (no stored session, or refresh itself failed/was revoked).</summary>
    public async Task<string?> GetValidAccessTokenAsync(CancellationToken ct = default)
    {
        _tokens ??= tokenStore.Load();
        if (_tokens is null)
            return null;

        if (!IsExpiringSoon(_tokens.AccessToken))
            return _tokens.AccessToken;

        try
        {
            _tokens = await apiClient.RefreshAsync(_tokens.RefreshToken, ct);
            tokenStore.Save(_tokens);
            return _tokens.AccessToken;
        }
        catch
        {
            _tokens = null;
            tokenStore.Clear();
            return null;
        }
    }

    private static bool IsExpiringSoon(string jwt)
    {
        var exp = TryGetExpiry(jwt);
        return exp is null || exp.Value <= DateTimeOffset.UtcNow.Add(ExpiryBuffer);
    }

    private static DateTimeOffset? TryGetExpiry(string jwt)
    {
        try
        {
            var payload = jwt.Split('.')[1].Replace('-', '+').Replace('_', '/');
            var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = JsonDocument.Parse(Convert.FromBase64String(padded));
            var exp = json.RootElement.GetProperty("exp").GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(exp);
        }
        catch
        {
            return null;
        }
    }
}
