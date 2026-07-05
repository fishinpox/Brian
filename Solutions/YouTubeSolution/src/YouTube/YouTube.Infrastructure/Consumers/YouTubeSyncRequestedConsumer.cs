using MassTransit;
using Microsoft.Extensions.Logging;
using YouTube.Application.Common.Events;

namespace YouTube.Infrastructure.Consumers;

public class YouTubeSyncRequestedConsumer(ILogger<YouTubeSyncRequestedConsumer> logger)
    : IConsumer<YouTubeSyncRequestedEvent>
{
    public Task Consume(ConsumeContext<YouTubeSyncRequestedEvent> context)
    {
        logger.LogInformation(
            "YouTube sync requested for profile {ProfileId} — not yet implemented", context.Message.ProfileId);
        return Task.CompletedTask;
    }
}
