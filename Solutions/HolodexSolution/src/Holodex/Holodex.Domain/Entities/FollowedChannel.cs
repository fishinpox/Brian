using Shared.Infrastructure.Common;

namespace Holodex.Domain.Entities;

public class FollowedChannel : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public string HolodexChannelId { get; private set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public string? PhotoUrl { get; set; }

    private FollowedChannel() { }

    public static FollowedChannel Create(
        Guid profileId,
        string holodexChannelId,
        string name,
        string? englishName,
        string? photoUrl)
    {
        return new FollowedChannel
        {
            ProfileId = profileId,
            HolodexChannelId = holodexChannelId,
            Name = name,
            EnglishName = englishName,
            PhotoUrl = photoUrl
        };
    }
}
