using Creator.Application.Common.DTOs;
using Creator.Application.Common.Interfaces;
using Creator.Domain.Entities;
using Creator.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Commands.ProvisionSite;

public class ProvisionSiteCommandHandler(ICreatorDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<ProvisionSiteCommand, Result<SiteDto>>
{
    public async Task<Result<SiteDto>> Handle(ProvisionSiteCommand request, CancellationToken cancellationToken)
    {
        var profileId = currentUser.ProfileId!.Value;

        // Idempotent: retrying provisioning for a profile that already owns a site just returns it.
        var existing = await db.Sites.FirstOrDefaultAsync(s => s.OwnerProfileId == profileId, cancellationToken);
        if (existing is not null)
            return Result<SiteDto>.Success(ToDto(existing, SiteAccessLevel.Admin));

        var site = Site.Provision(profileId, request.RequestedSlug);
        db.Sites.Add(site);
        db.SiteAccessGrants.Add(SiteAccessGrant.Grant(site.Id, profileId, SiteAccessLevel.Admin, profileId));
        await db.SaveChangesAsync(cancellationToken);

        return Result<SiteDto>.Success(ToDto(site, SiteAccessLevel.Admin));
    }

    private static SiteDto ToDto(Site site, SiteAccessLevel myAccessLevel) => new(
        site.Id, site.OwnerProfileId, site.Slug, site.PublishState, site.PublishedAt, site.CreatedAt, myAccessLevel);
}
