using Creator.Application.Common.DTOs;
using Creator.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Queries.GetMySite;

public class GetMySiteQueryHandler(ICreatorDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMySiteQuery, Result<SiteDto>>
{
    public async Task<Result<SiteDto>> Handle(GetMySiteQuery request, CancellationToken cancellationToken)
    {
        var profileId = currentUser.ProfileId!.Value;

        var grant = await db.SiteAccessGrants
            .Where(g => g.ProfileId == profileId && g.RevokedAt == null)
            .OrderByDescending(g => g.AccessLevel)
            .FirstOrDefaultAsync(cancellationToken);

        if (grant is null)
            return Result<SiteDto>.Failure("No site found for this profile.");

        var site = await db.Sites.FirstAsync(s => s.Id == grant.SiteId, cancellationToken);

        return Result<SiteDto>.Success(new SiteDto(
            site.Id, site.OwnerProfileId, site.Slug, site.PublishState, site.PublishedAt, site.CreatedAt, grant.AccessLevel));
    }
}
