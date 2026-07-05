using Calendar.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class Reminder : BaseEntity
{
    public Guid PersonalEventId { get; private set; }
    public Guid ProfileId { get; private set; }
    public DateTimeOffset TriggerAt { get; private set; }
    public ReminderMethod Method { get; private set; }
    public bool IsSent { get; set; }
    public DateTimeOffset? SentAt { get; set; }

    private Reminder() { }

    public static Reminder Create(
        Guid personalEventId,
        Guid profileId,
        DateTimeOffset triggerAt,
        ReminderMethod method)
    {
        return new Reminder
        {
            PersonalEventId = personalEventId,
            ProfileId = profileId,
            TriggerAt = triggerAt,
            Method = method,
            IsSent = false
        };
    }
}
