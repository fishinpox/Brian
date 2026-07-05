using Refit;

namespace YouTube.Application.Common.Interfaces;

public record YouTubeSearchItemIdDto(string VideoId);

public record YouTubeSearchItemSnippetDto(string ChannelId, string Title, string LiveBroadcastContent);

public record YouTubeSearchItemDto(YouTubeSearchItemIdDto Id, YouTubeSearchItemSnippetDto Snippet);

public record YouTubeSearchResponse(List<YouTubeSearchItemDto> Items);

public interface IYouTubeApiClient
{
    [Get("/youtube/v3/search")]
    Task<YouTubeSearchResponse> GetLiveBroadcastsAsync(
        [Query] string part,
        [Query] string channelId,
        [Query] string eventType,
        [Query] string key);
}
