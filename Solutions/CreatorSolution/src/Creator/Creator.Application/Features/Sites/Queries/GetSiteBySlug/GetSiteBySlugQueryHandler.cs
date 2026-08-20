using Creator.Application.Common.DTOs;
using Creator.Application.Common.Interfaces;
using Creator.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Queries.GetSiteBySlug;

/// <summary>
/// Backs anonymous/public lookups only (the vanity-path catch-all and its API twin) - a
/// Draft or unpublished site must be indistinguishable from one that doesn't exist, per the
/// "nothing reaches the public site until it is explicitly published" requirement. Owners
/// checking their own site's status (regardless of publish state) go through GetMySiteQuery.
/// </summary>
public class GetSiteBySlugQueryHandler(ICreatorDbContext db)
    : IRequestHandler<GetSiteBySlugQuery, Result<PublicSiteDto>>
{
    public async Task<Result<PublicSiteDto>> Handle(GetSiteBySlugQuery request, CancellationToken cancellationToken)
    {
        var site = await db.Sites.FirstOrDefaultAsync(
            s => s.Slug == request.Slug && s.PublishState == SitePublishState.Published, cancellationToken);
        if (site is null)
            return Result<PublicSiteDto>.Failure("No site found for that address.");

        return Result<PublicSiteDto>.Success(new PublicSiteDto(site.Slug, site.PublishState));
    }
}
