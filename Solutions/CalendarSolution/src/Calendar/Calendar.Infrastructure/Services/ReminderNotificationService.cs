using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Calendar.Infrastructure.Services;

public class ReminderNotificationService(
    IHttpClientFactory httpClientFactory,
    ILogger<ReminderNotificationService> logger)
    : IReminderNotificationService
{
    public async Task SendReminderAsync(Guid profileId, string eventTitle, ReminderMethod method, CancellationToken ct)
    {
        try
        {
            var client = httpClientFactory.CreateClient("notifications");
            var payload = new { ProfileId = profileId, EventTitle = eventTitle, Method = method.ToString() };
            var response = await client.PostAsJsonAsync("/api/notifications/dispatch/reminder", payload, ct);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to dispatch reminder notification for profile {ProfileId}: {EventTitle}",
                profileId, eventTitle);
        }
    }
}
