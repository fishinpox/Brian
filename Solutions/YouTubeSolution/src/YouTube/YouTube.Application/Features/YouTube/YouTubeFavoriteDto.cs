namespace YouTube.Application.Features.YouTube;

public record YouTubeFavoriteDto(
    string ChannelId,
    string Name,
    string? EnglishName,
    string? PhotoUrl,
    bool IsFollowed = false);
