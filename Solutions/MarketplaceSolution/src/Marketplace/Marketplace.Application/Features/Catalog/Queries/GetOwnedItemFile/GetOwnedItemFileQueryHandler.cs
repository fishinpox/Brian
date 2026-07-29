using Marketplace.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Marketplace.Application.Features.Catalog.Queries.GetOwnedItemFile;

public class GetOwnedItemFileQueryHandler(IMarketplaceDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetOwnedItemFileQuery, Result<WallpaperFileDto>>
{
    public async Task<Result<WallpaperFileDto>> Handle(GetOwnedItemFileQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var owns = await db.WallpaperOwnerships
            .AnyAsync(o => o.ProfileId == currentUser.ProfileId.Value && o.ItemId == request.ItemId, cancellationToken);

        if (!owns)
            throw new ForbiddenAccessException();

        var item = await db.WallpaperItems
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

        return item is null
            ? Result<WallpaperFileDto>.Failure("Wallpaper not found.")
            : Result<WallpaperFileDto>.Success(new WallpaperFileDto(item.ImageData, item.ContentType, item.FileName));
    }
}
