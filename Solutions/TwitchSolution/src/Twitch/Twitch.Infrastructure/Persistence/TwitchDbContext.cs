using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Twitch.Application.Common.Interfaces;
using Twitch.Domain.Entities;

namespace Twitch.Infrastructure.Persistence;

public class TwitchDbContext(DbContextOptions<TwitchDbContext> options)
    : DbContext(options), ITwitchDbContext
{
    public DbSet<ExternalCredential> ExternalCredentials => Set<ExternalCredential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TwitchDbContext).Assembly);
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
