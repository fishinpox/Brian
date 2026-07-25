using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notifications.API.Hubs;
using Shared.Contracts.Events.Moderation;

namespace Notifications.API.Consumers;

public class CalendarBackgroundApprovedConsumer(IHubContext<NotificationHub> hub, ILogger<CalendarBackgroundApprovedConsumer> logger)
    : IConsumer<CalendarBackgroundApprovedEvent>
{
    public async Task Consume(ConsumeContext<CalendarBackgroundApprovedEvent> context)
    {
        var ev = context.Message;
        logger.LogInformation("CalendarBackgroundApproved: profile {ProfileId}", ev.ProfileId);

        await hub.Clients
            .Group(ev.ProfileId.ToString())
            .SendAsync("calendar-background-ready", new { ev.SubmissionId });
    }
}
