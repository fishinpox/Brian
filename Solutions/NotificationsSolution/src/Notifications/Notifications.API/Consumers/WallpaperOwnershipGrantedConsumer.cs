using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notifications.API.Hubs;
using Shared.Contracts.Events.Marketplace;

namespace Notifications.API.Consumers;

public class WallpaperOwnershipGrantedConsumer(IHubContext<NotificationHub> hub, ILogger<WallpaperOwnershipGrantedConsumer> logger)
    : IConsumer<WallpaperOwnershipGrantedEvent>
{
    public async Task Consume(ConsumeContext<WallpaperOwnershipGrantedEvent> context)
    {
        var ev = context.Message;
        logger.LogInformation("WallpaperOwnershipGranted: profile {ProfileId}, item {ItemId}", ev.ProfileId, ev.ItemId);

        await hub.Clients
            .Group(ev.ProfileId.ToString())
            .SendAsync("wallpaper-owned", new { ev.OwnershipId, ev.ItemId });
    }
}
