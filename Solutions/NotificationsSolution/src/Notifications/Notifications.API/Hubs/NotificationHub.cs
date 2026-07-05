using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Notifications.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var profileId = Context.User?.FindFirst("profile_id")?.Value;
        if (profileId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, profileId);
        await base.OnConnectedAsync();
    }
}
