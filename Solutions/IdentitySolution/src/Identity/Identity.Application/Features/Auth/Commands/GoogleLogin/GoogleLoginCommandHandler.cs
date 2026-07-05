using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.GoogleLogin;

public class GoogleLoginCommandHandler(
    IIdentityDbContext db,
    IAccountService accountService,
    ITokenService tokenService,
    IHttpClientFactory httpClientFactory,
    IConfiguration config)
    : IRequestHandler<GoogleLoginCommand, Result<GoogleLoginResponse>>
{
    public async Task<Result<GoogleLoginResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var tokenResponse = await ExchangeCodeForTokensAsync(request.Code, cancellationToken);
        if (tokenResponse is null)
            return Result<GoogleLoginResponse>.Failure("Failed to exchange authorization code with Google.");

        var googleUser = ExtractGoogleUser(tokenResponse.IdToken);
        if (googleUser is null)
            return Result<GoogleLoginResponse>.Failure("Failed to read Google user information.");

        var existingOAuth = await db.OAuthIdentities
            .FirstOrDefaultAsync(o => o.Provider == Platform.Google && o.ProviderUserId == googleUser.Sub, cancellationToken);

        if (existingOAuth is not null)
            return await HandleReturningUserAsync(existingOAuth, tokenResponse, cancellationToken);

        return await HandleNewUserAsync(googleUser, tokenResponse, cancellationToken);
    }

    private async Task<Result<GoogleLoginResponse>> HandleReturningUserAsync(
        OAuthIdentity oAuth, GoogleTokenResponse tokenResponse, CancellationToken ct)
    {
        oAuth.EncryptedAccessToken = tokenResponse.AccessToken;
        oAuth.EncryptedRefreshToken = tokenResponse.RefreshToken;
        oAuth.TokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
        await db.SaveChangesAsync(ct);

        var profile = await db.Profiles
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.AccountId == oAuth.AccountId, ct);

        if (profile is null)
            return Result<GoogleLoginResponse>.Success(
                new GoogleLoginResponse(oAuth.AccountId, null, tokenService.GenerateAccountOnlyToken(oAuth.AccountId), true));

        var token = tokenService.GenerateToken(oAuth.AccountId, profile);
        return Result<GoogleLoginResponse>.Success(
            new GoogleLoginResponse(oAuth.AccountId, profile.Id, token, false));
    }

    private async Task<Result<GoogleLoginResponse>> HandleNewUserAsync(
        GoogleUserInfo googleUser, GoogleTokenResponse tokenResponse, CancellationToken ct)
    {
        var password = Guid.NewGuid().ToString("N") + "Aa1!";
        var (success, accountId, errors) = await accountService.CreateAccountAsync(googleUser.Email, password, ct);
        if (!success)
            return Result<GoogleLoginResponse>.Failure(errors);

        var oAuthIdentity = new OAuthIdentity
        {
            AccountId = accountId,
            Provider = Platform.Google,
            ProviderUserId = googleUser.Sub,
            EncryptedAccessToken = tokenResponse.AccessToken,
            EncryptedRefreshToken = tokenResponse.RefreshToken,
            TokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
        };

        db.OAuthIdentities.Add(oAuthIdentity);
        await db.SaveChangesAsync(ct);

        var token = tokenService.GenerateAccountOnlyToken(accountId);
        return Result<GoogleLoginResponse>.Success(
            new GoogleLoginResponse(accountId, null, token, true));
    }

    private async Task<GoogleTokenResponse?> ExchangeCodeForTokensAsync(string code, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient("Google");
        var response = await client.PostAsync("https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = config["Google:ClientId"]!,
                ["client_secret"] = config["Google:ClientSecret"]!,
                ["redirect_uri"] = config["Google:RedirectUri"]!,
                ["grant_type"] = "authorization_code"
            }), ct);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GoogleTokenResponse>(ct);
    }

    private static GoogleUserInfo? ExtractGoogleUser(string idToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(idToken))
            return null;

        var jwt = handler.ReadJwtToken(idToken);
        var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        var picture = jwt.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

        if (sub is null || email is null)
            return null;

        return new GoogleUserInfo(sub, email, name ?? email, picture);
    }
}

record GoogleTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("id_token")] string IdToken,
    [property: JsonPropertyName("refresh_token")] string? RefreshToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType);

record GoogleUserInfo(string Sub, string Email, string Name, string? Picture);
