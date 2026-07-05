using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Notifications.API.Hubs;

namespace Notifications.API.Controllers;

public record DispatchReminderRequest(Guid ProfileId, string EventTitle, string Method);

[ApiController]
[Route("api/notifications")]
public class NotificationsController(
    IHubContext<NotificationHub> hub,
    ILogger<NotificationsController> logger) : ControllerBase
{
    [HttpPost("dispatch/reminder")]
    [AllowAnonymous]
    public async Task<IActionResult> DispatchReminder(
        [FromBody] DispatchReminderRequest request,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Dispatching reminder [{Method}] for profile {ProfileId}: {EventTitle}",
            request.Method, request.ProfileId, request.EventTitle);

        await hub.Clients
            .Group(request.ProfileId.ToString())
            .SendAsync("reminder", new { request.EventTitle, request.Method }, ct);

        return Ok();
    }
}
