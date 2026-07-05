namespace Holodex.Application.Features.Holodex;

public record HolodexFavoriteDto(
    string ChannelId,
    string Name,
    string? EnglishName,
    string? PhotoUrl,
    bool IsFollowed = false);
