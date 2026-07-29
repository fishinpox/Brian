using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Common.Interfaces;

public interface IMarketplaceDbContext
{
    DbSet<WallpaperItem> WallpaperItems { get; }
    DbSet<WallpaperOwnership> WallpaperOwnerships { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
