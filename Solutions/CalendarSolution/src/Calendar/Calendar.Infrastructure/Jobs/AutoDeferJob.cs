using Calendar.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Calendar.Infrastructure.Jobs;

public class AutoDeferJob(ICalendarDbContext db, ILogger<AutoDeferJob> logger)
{
    private const int MaxConsecutiveDefers = 7;
    private const int LookBackDays = 3;

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var today = DateTimeOffset.UtcNow.Date;
        var lookBackStart = today.AddDays(-LookBackDays);

        var candidates = await db.PersonalEvents
            .Where(e => !e.IsCompleted
                && e.AutoDeferEnabled
                && e.IsAllDay
                && e.DeferCount < MaxConsecutiveDefers
                && e.StartAt >= lookBackStart
                && e.StartAt < today)
            .Take(500)
            .ToListAsync(ct);

        foreach (var personalEvent in candidates)
        {
            personalEvent.DeferToDate(today);
        }

        if (candidates.Count > 0)
        {
            logger.LogInformation("Auto-deferred {Count} incomplete all-day events to today", candidates.Count);
        }

        await db.SaveChangesAsync(ct);
    }
}
