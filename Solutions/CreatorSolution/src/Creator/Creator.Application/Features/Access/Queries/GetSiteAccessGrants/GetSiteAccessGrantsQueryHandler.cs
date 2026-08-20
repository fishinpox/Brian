using Creator.Application.Common;
using Creator.Application.Common.DTOs;
using Creator.Application.Common.Interfaces;
using Creator.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Access.Queries.GetSiteAccessGrants;

public class GetSiteAccessGrantsQueryHandler(ICreatorDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetSiteAccessGrantsQuery, Result<List<SiteAccessGrantDto>>>
{
    public async Task<Result<List<SiteAccessGrantDto>>> Handle(GetSiteAccessGrantsQuery request, CancellationToken cancellationToken)
    {
        var callerProfileId = currentUser.ProfileId!.Value;
        var callerAccess = await SiteAccessGuard.GetAccessLevelAsync(db, request.SiteId, callerProfileId, cancellationToken);
        if (callerAccess != SiteAccessLevel.Admin)
            return Result<List<SiteAccessGrantDto>>.Failure("Only the site's Admin can view its access grants.");

        var grants = await db.SiteAccessGrants
            .Where(g => g.SiteId == request.SiteId && g.RevokedAt == null)
            .OrderBy(g => g.GrantedAt)
            .Select(g => new SiteAccessGrantDto(g.Id, g.SiteId, g.ProfileId, g.AccessLevel, g.GrantedAt, g.GrantedByProfileId))
            .ToListAsync(cancellationToken);

        return Result<List<SiteAccessGrantDto>>.Success(grants);
    }
}
