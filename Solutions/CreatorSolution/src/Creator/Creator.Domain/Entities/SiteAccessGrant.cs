using Creator.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Creator.Domain.Entities;

public class SiteAccessGrant : BaseAuditableEntity
{
    public Guid SiteId { get; private set; }
    public Guid ProfileId { get; private set; }
    public SiteAccessLevel AccessLevel { get; private set; }
    public DateTimeOffset GrantedAt { get; private set; }
    public Guid GrantedByProfileId { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    private SiteAccessGrant() { }

    public static SiteAccessGrant Grant(Guid siteId, Guid profileId, SiteAccessLevel accessLevel, Guid grantedByProfileId)
    {
        return new SiteAccessGrant
        {
            SiteId = siteId,
            ProfileId = profileId,
            AccessLevel = accessLevel,
            GrantedAt = DateTimeOffset.UtcNow,
            GrantedByProfileId = grantedByProfileId,
            RevokedAt = null
        };
    }

    public void ChangeLevel(SiteAccessLevel accessLevel) => AccessLevel = accessLevel;

    public void Revoke() => RevokedAt = DateTimeOffset.UtcNow;
}
