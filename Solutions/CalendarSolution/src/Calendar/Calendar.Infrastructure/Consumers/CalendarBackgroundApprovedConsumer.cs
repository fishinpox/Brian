using Calendar.Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events.Moderation;

namespace Calendar.Infrastructure.Consumers;

public class CalendarBackgroundApprovedConsumer(ICalendarDbContext db, ILogger<CalendarBackgroundApprovedConsumer> logger)
    : IConsumer<CalendarBackgroundApprovedEvent>
{
    public async Task Consume(ConsumeContext<CalendarBackgroundApprovedEvent> context)
    {
        var ev = context.Message;

        var background = await db.CalendarBackgrounds
            .FirstOrDefaultAsync(b => b.Id == ev.SubmissionId, context.CancellationToken);

        if (background is null)
        {
            logger.LogWarning("CalendarBackgroundApproved: submission {SubmissionId} not found", ev.SubmissionId);
            return;
        }

        background.Approve();
        await db.SaveChangesAsync(context.CancellationToken);
    }
}
