using Creator.Application.Common.Interfaces;
using Creator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Creator.Application.Common;

public static class SiteAccessGuard
{
    public static async Task<SiteAccessLevel?> GetAccessLevelAsync(
        ICreatorDbContext db, Guid siteId, Guid profileId, CancellationToken cancellationToken)
    {
        var grant = await db.SiteAccessGrants
            .Where(g => g.SiteId == siteId && g.ProfileId == profileId && g.RevokedAt == null)
            .FirstOrDefaultAsync(cancellationToken);

        return grant?.AccessLevel;
    }
}
