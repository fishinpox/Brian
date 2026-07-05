using Refit;

namespace Twitch.Application.Common.Interfaces;

public record TwitchStreamDto(
    string Id,
    string UserId,
    string UserName,
    string Title,
    string GameName,
    string ThumbnailUrl,
    string StartedAt);

public record TwitchStreamsResponse(List<TwitchStreamDto> Data);

public interface ITwitchApiClient
{
    [Get("/helix/streams")]
    Task<TwitchStreamsResponse> GetLiveStreamsAsync(
        [Header("Authorization")] string bearerToken,
        [Header("Client-Id")] string clientId,
        [Query] string user_login);
}
