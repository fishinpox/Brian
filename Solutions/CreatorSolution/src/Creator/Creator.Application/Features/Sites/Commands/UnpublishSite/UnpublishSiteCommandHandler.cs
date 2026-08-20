using Creator.Application.Common;
using Creator.Application.Common.DTOs;
using Creator.Application.Common.Interfaces;
using Creator.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Commands.UnpublishSite;

public class UnpublishSiteCommandHandler(ICreatorDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UnpublishSiteCommand, Result<SiteDto>>
{
    public async Task<Result<SiteDto>> Handle(UnpublishSiteCommand request, CancellationToken cancellationToken)
    {
        var profileId = currentUser.ProfileId!.Value;
        var accessLevel = await SiteAccessGuard.GetAccessLevelAsync(db, request.SiteId, profileId, cancellationToken);
        if (accessLevel != SiteAccessLevel.Admin)
            return Result<SiteDto>.Failure("Only the site's Admin can unpublish it.");

        var site = await db.Sites.FirstOrDefaultAsync(s => s.Id == request.SiteId, cancellationToken);
        if (site is null)
            return Result<SiteDto>.Failure("Site not found.");

        site.Unpublish();
        await db.SaveChangesAsync(cancellationToken);

        return Result<SiteDto>.Success(new SiteDto(
            site.Id, site.OwnerProfileId, site.Slug, site.PublishState, site.PublishedAt, site.CreatedAt, accessLevel.Value));
    }
}
