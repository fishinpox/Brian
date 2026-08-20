using Creator.Application.Common.Interfaces;
using Creator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Creator.Infrastructure.Persistence;

public class CreatorDbContext(DbContextOptions<CreatorDbContext> options)
    : DbContext(options), ICreatorDbContext
{
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<SiteAccessGrant> SiteAccessGrants => Set<SiteAccessGrant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreatorDbContext).Assembly);
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
