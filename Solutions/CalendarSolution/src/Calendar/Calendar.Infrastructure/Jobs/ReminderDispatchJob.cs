using Calendar.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Calendar.Infrastructure.Jobs;

public class ReminderDispatchJob(
    ICalendarDbContext db,
    IReminderNotificationService notifier,
    ILogger<ReminderDispatchJob> logger)
{
    public async Task ExecuteAsync(CancellationToken ct)
    {
        var dueReminders = await db.Reminders
            .Where(r => !r.IsSent && r.TriggerAt <= DateTimeOffset.UtcNow)
            .Take(100)
            .ToListAsync(ct);

        foreach (var reminder in dueReminders)
        {
            try
            {
                await notifier.SendReminderAsync(reminder.ProfileId, "Event Reminder", reminder.Method, ct);
                reminder.IsSent = true;
                reminder.SentAt = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send reminder {ReminderId}", reminder.Id);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
