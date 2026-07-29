using System.Net.Http.Headers;
using System.Net.Http.Json;
using NativeApp.Core.Models;

namespace NativeApp.Core;

/// <summary>Thin wrapper over the backend HTTP surface. Takes an explicit access token per call
/// rather than holding its own auth state - token lifecycle lives in AuthSession.</summary>
public class ApiClient(HttpClient http)
{
    private record LoginRequest(string Email, string Password);
    private record RefreshRequest(string RefreshToken);
    private record LoginResponse(Guid AccountId, Guid ProfileId, string Token, string RefreshToken, string[] Roles);
    private record RefreshResponse(Guid AccountId, Guid ProfileId, string Token, string RefreshToken, string[] Roles);

    public async Task<AuthTokens> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var res = await http.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password), ct);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<LoginResponse>(ct)
            ?? throw new InvalidOperationException("Login response was empty.");
        return new AuthTokens(body.Token, body.RefreshToken);
    }

    public async Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var res = await http.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(refreshToken), ct);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<RefreshResponse>(ct)
            ?? throw new InvalidOperationException("Refresh response was empty.");
        return new AuthTokens(body.Token, body.RefreshToken);
    }

    public async Task LogoutAsync(string accessToken, string refreshToken, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout")
        {
            Content = JsonContent.Create(new RefreshRequest(refreshToken))
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        await http.SendAsync(request, ct);
    }

    public async Task<List<CatalogItem>> GetCatalogAsync(string accessToken, CancellationToken ct = default)
    {
        var request = Authorized(HttpMethod.Get, "/api/marketplace/items", accessToken);
        var res = await http.SendAsync(request, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<List<CatalogItem>>(ct) ?? [];
    }

    public async Task PurchaseAsync(string accessToken, Guid itemId, CancellationToken ct = default)
    {
        var request = Authorized(HttpMethod.Post, $"/api/marketplace/items/{itemId}/purchase", accessToken);
        var res = await http.SendAsync(request, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task<List<PendingOwnership>> GetPendingOwnershipsAsync(string accessToken, CancellationToken ct = default)
    {
        var request = Authorized(HttpMethod.Get, "/api/marketplace/ownership/pending", accessToken);
        var res = await http.SendAsync(request, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<List<PendingOwnership>>(ct) ?? [];
    }

    public async Task<WallpaperFile> DownloadItemFileAsync(string accessToken, Guid itemId, CancellationToken ct = default)
    {
        var request = Authorized(HttpMethod.Get, $"/api/marketplace/items/{itemId}/file", accessToken);
        var res = await http.SendAsync(request, ct);
        res.EnsureSuccessStatusCode();

        var bytes = await res.Content.ReadAsByteArrayAsync(ct);
        var contentType = res.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = res.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"{itemId}.dat";
        return new WallpaperFile(bytes, contentType, fileName);
    }

    public async Task MarkAppliedAsync(string accessToken, Guid ownershipId, CancellationToken ct = default)
    {
        var request = Authorized(HttpMethod.Post, $"/api/marketplace/ownership/{ownershipId}/mark-applied", accessToken);
        var res = await http.SendAsync(request, ct);
        res.EnsureSuccessStatusCode();
    }

    private static HttpRequestMessage Authorized(HttpMethod method, string path, string accessToken)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return request;
    }
}
