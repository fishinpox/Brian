using Calendar.Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events.Moderation;

namespace Calendar.Infrastructure.Consumers;

public class CalendarBackgroundRejectedConsumer(ICalendarDbContext db, ILogger<CalendarBackgroundRejectedConsumer> logger)
    : IConsumer<CalendarBackgroundRejectedEvent>
{
    public async Task Consume(ConsumeContext<CalendarBackgroundRejectedEvent> context)
    {
        var ev = context.Message;

        var background = await db.CalendarBackgrounds
            .FirstOrDefaultAsync(b => b.Id == ev.SubmissionId, context.CancellationToken);

        if (background is null)
        {
            logger.LogWarning("CalendarBackgroundRejected: submission {SubmissionId} not found", ev.SubmissionId);
            return;
        }

        background.Reject(ev.Reason);
        await db.SaveChangesAsync(context.CancellationToken);

        // TODO: email the user the rejection reason once Notifications service can resolve profile -> email.
        logger.LogInformation(
            "CalendarBackground {SubmissionId} rejected for profile {ProfileId}: {Reason}",
            ev.SubmissionId, ev.ProfileId, ev.Reason);
    }
}
