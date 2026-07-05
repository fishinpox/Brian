using Calendar.Domain.Enums;

namespace Calendar.Application.Common.Interfaces;

public interface IReminderNotificationService
{
    Task SendReminderAsync(Guid profileId, string eventTitle, ReminderMethod method, CancellationToken ct);
}
