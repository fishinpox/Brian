using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Calendar.Infrastructure.Persistence;

public class CalendarDbContext(DbContextOptions<CalendarDbContext> options)
    : DbContext(options), ICalendarDbContext
{
    public DbSet<PersonalEvent> PersonalEvents => Set<PersonalEvent>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<StreamEvent> StreamEvents => Set<StreamEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalendarDbContext).Assembly);
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
