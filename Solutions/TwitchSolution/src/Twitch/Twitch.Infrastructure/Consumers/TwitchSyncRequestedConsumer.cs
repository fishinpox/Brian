using MassTransit;
using Microsoft.Extensions.Logging;
using Twitch.Application.Common.Events;

namespace Twitch.Infrastructure.Consumers;

public class TwitchSyncRequestedConsumer(ILogger<TwitchSyncRequestedConsumer> logger)
    : IConsumer<TwitchSyncRequestedEvent>
{
    public Task Consume(ConsumeContext<TwitchSyncRequestedEvent> context)
    {
        logger.LogInformation(
            "Twitch sync requested for profile {ProfileId} — not yet implemented", context.Message.ProfileId);
        return Task.CompletedTask;
    }
}
