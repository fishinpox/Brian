using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using YouTube.Application.Common.Interfaces;
using YouTube.Domain.Entities;

namespace YouTube.Infrastructure.Persistence;

public class YouTubeDbContext(DbContextOptions<YouTubeDbContext> options)
    : DbContext(options), IYouTubeDbContext
{
    public DbSet<ExternalCredential> ExternalCredentials => Set<ExternalCredential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(YouTubeDbContext).Assembly);
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
