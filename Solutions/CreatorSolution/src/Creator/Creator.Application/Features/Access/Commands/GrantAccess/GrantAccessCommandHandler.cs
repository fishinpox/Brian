using Creator.Application.Common;
using Creator.Application.Common.DTOs;
using Creator.Application.Common.Interfaces;
using Creator.Domain.Entities;
using Creator.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Access.Commands.GrantAccess;

public class GrantAccessCommandHandler(ICreatorDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GrantAccessCommand, Result<SiteAccessGrantDto>>
{
    public async Task<Result<SiteAccessGrantDto>> Handle(GrantAccessCommand request, CancellationToken cancellationToken)
    {
        var callerProfileId = currentUser.ProfileId!.Value;
        var callerAccess = await SiteAccessGuard.GetAccessLevelAsync(db, request.SiteId, callerProfileId, cancellationToken);
        if (callerAccess != SiteAccessLevel.Admin)
            return Result<SiteAccessGrantDto>.Failure("Only the site's Admin can grant access.");

        var existingActive = await db.SiteAccessGrants.FirstOrDefaultAsync(
            g => g.SiteId == request.SiteId && g.ProfileId == request.GranteeProfileId && g.RevokedAt == null,
            cancellationToken);

        SiteAccessGrant grant;
        if (existingActive is not null)
        {
            existingActive.ChangeLevel(request.AccessLevel);
            grant = existingActive;
        }
        else
        {
            grant = SiteAccessGrant.Grant(request.SiteId, request.GranteeProfileId, request.AccessLevel, callerProfileId);
            db.SiteAccessGrants.Add(grant);
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result<SiteAccessGrantDto>.Success(new SiteAccessGrantDto(
            grant.Id, grant.SiteId, grant.ProfileId, grant.AccessLevel, grant.GrantedAt, grant.GrantedByProfileId));
    }
}
