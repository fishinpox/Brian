using Creator.Application.Common.DTOs;
using Creator.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Queries.GetSiteBySlug;

public class GetSiteBySlugQueryHandler(ICreatorDbContext db)
    : IRequestHandler<GetSiteBySlugQuery, Result<PublicSiteDto>>
{
    public async Task<Result<PublicSiteDto>> Handle(GetSiteBySlugQuery request, CancellationToken cancellationToken)
    {
        var site = await db.Sites.FirstOrDefaultAsync(s => s.Slug == request.Slug, cancellationToken);
        if (site is null)
            return Result<PublicSiteDto>.Failure("No site found for that address.");

        return Result<PublicSiteDto>.Success(new PublicSiteDto(site.Slug, site.PublishState));
    }
}
