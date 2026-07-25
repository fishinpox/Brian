using Shared.Infrastructure.Common;

namespace YouTube.Domain.Entities;

public class FollowedChannel : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public string YouTubeChannelId { get; private set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public string? PhotoUrl { get; set; }

    private FollowedChannel() { }

    public static FollowedChannel Create(
        Guid profileId,
        string youTubeChannelId,
        string name,
        string? englishName,
        string? photoUrl)
    {
        return new FollowedChannel
        {
            ProfileId = profileId,
            YouTubeChannelId = youTubeChannelId,
            Name = name,
            EnglishName = englishName,
            PhotoUrl = photoUrl
        };
    }
}
