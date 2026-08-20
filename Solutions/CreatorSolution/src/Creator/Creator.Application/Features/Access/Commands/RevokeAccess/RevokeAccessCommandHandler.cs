using Creator.Application.Common;
using Creator.Application.Common.Interfaces;
using Creator.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Access.Commands.RevokeAccess;

public class RevokeAccessCommandHandler(ICreatorDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<RevokeAccessCommand, Result>
{
    public async Task<Result> Handle(RevokeAccessCommand request, CancellationToken cancellationToken)
    {
        var callerProfileId = currentUser.ProfileId!.Value;
        var callerAccess = await SiteAccessGuard.GetAccessLevelAsync(db, request.SiteId, callerProfileId, cancellationToken);
        if (callerAccess != SiteAccessLevel.Admin)
            return Result.Failure("Only the site's Admin can revoke access.");

        var site = await db.Sites.FirstOrDefaultAsync(s => s.Id == request.SiteId, cancellationToken);
        if (site is null)
            return Result.Failure("Site not found.");

        if (site.OwnerProfileId == request.GranteeProfileId)
            return Result.Failure("Cannot revoke the site owner's own access.");

        var grant = await db.SiteAccessGrants.FirstOrDefaultAsync(
            g => g.SiteId == request.SiteId && g.ProfileId == request.GranteeProfileId && g.RevokedAt == null,
            cancellationToken);
        if (grant is null)
            return Result.Failure("No active access grant found for that profile.");

        grant.Revoke();
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
