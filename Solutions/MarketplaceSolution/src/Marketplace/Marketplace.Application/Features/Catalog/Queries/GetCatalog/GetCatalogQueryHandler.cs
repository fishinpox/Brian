using Marketplace.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Marketplace.Application.Features.Catalog.Queries.GetCatalog;

public class GetCatalogQueryHandler(IMarketplaceDbContext db)
    : IRequestHandler<GetCatalogQuery, Result<List<WallpaperItemDto>>>
{
    public async Task<Result<List<WallpaperItemDto>>> Handle(GetCatalogQuery request, CancellationToken cancellationToken)
    {
        var items = await db.WallpaperItems
            .Where(i => i.IsActive)
            .Select(i => new WallpaperItemDto(i.Id, i.Name, i.Description, i.Price))
            .ToListAsync(cancellationToken);

        return Result<List<WallpaperItemDto>>.Success(items);
    }
}
