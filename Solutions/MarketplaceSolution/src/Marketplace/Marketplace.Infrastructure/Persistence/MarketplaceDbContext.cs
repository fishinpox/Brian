using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Marketplace.Infrastructure.Persistence;

public class MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
    : DbContext(options), IMarketplaceDbContext
{
    public DbSet<WallpaperItem> WallpaperItems => Set<WallpaperItem>();
    public DbSet<WallpaperOwnership> WallpaperOwnerships => Set<WallpaperOwnership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketplaceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
