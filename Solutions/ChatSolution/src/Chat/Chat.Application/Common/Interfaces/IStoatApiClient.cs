using System.Text.Json.Serialization;
using Refit;

namespace Chat.Application.Common.Interfaces;

public record StoatCreateAccountRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password);

public record StoatLoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("friendly_name")] string FriendlyName);

public record StoatLoginResponse(
    [property: JsonPropertyName("user_id")] string UserId,
    [property: JsonPropertyName("token")] string Token);

public record StoatOnboardCompleteRequest(
    [property: JsonPropertyName("username")] string Username);

public record StoatOnboardCompleteResponse(
    [property: JsonPropertyName("_id")] string Id,
    [property: JsonPropertyName("username")] string Username);

/// <summary>
/// Confirmed against a real self-hosted instance during the Chat window spike (see
/// Documentation/Calendar/ChatWindow.md plan): account creation and login both work fully
/// server-side with no email verification or captcha (this instance has both disabled), but
/// new accounts land in an onboarding state until a username is set - regular endpoints 401
/// until onboarding completes.
/// </summary>
public interface IStoatApiClient
{
    [Post("/auth/account/create")]
    Task CreateAccountAsync([Body] StoatCreateAccountRequest request);

    [Post("/auth/session/login")]
    Task<StoatLoginResponse> LoginAsync([Body] StoatLoginRequest request);

    [Post("/onboard/complete")]
    Task<StoatOnboardCompleteResponse> CompleteOnboardingAsync(
        [Header("x-session-token")] string sessionToken,
        [Body] StoatOnboardCompleteRequest request);
}
