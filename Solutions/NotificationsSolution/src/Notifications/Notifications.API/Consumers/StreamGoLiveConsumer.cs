using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notifications.API.Hubs;
using Shared.Contracts.Events.Calendar;

namespace Notifications.API.Consumers;

public class StreamGoLiveConsumer(IHubContext<NotificationHub> hub, ILogger<StreamGoLiveConsumer> logger)
    : IConsumer<StreamGoLiveEvent>
{
    public async Task Consume(ConsumeContext<StreamGoLiveEvent> context)
    {
        var ev = context.Message;
        logger.LogInformation("StreamGoLive: {Title} by VTuber {VTuberProfileId}", ev.Title, ev.VTuberProfileId);

        await hub.Clients
            .Group(ev.VTuberProfileId.ToString())
            .SendAsync("stream-go-live", new
            {
                ev.StreamEventId,
                ev.Title,
                ev.Platform,
                ev.ThumbnailUrl,
                ev.StartedAt
            });
    }
}
