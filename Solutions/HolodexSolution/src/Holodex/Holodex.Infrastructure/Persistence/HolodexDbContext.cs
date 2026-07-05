using Holodex.Application.Common.Interfaces;
using Holodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Holodex.Infrastructure.Persistence;

public class HolodexDbContext(DbContextOptions<HolodexDbContext> options)
    : DbContext(options), IHolodexDbContext
{
    public DbSet<ExternalCredential> ExternalCredentials => Set<ExternalCredential>();
    public DbSet<FollowedChannel> FollowedChannels => Set<FollowedChannel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HolodexDbContext).Assembly);
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
