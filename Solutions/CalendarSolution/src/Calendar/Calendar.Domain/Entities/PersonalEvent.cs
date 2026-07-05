using Calendar.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class PersonalEvent : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public DateTimeOffset StartAt { get; private set; }
    public DateTimeOffset? EndAt { get; private set; }
    public bool IsAllDay { get; private set; }
    public string? RecurrenceRule { get; private set; }
    public EventStatus Status { get; private set; }

    private PersonalEvent() { }

    public static PersonalEvent Create(
        Guid profileId,
        string title,
        string? description,
        string? location,
        DateTimeOffset startAt,
        DateTimeOffset? endAt,
        bool isAllDay,
        string? recurrenceRule)
    {
        if (endAt.HasValue && endAt.Value <= startAt)
            throw new ArgumentException("EndAt must be after StartAt.");

        return new PersonalEvent
        {
            ProfileId = profileId,
            Title = title,
            Description = description,
            Location = location,
            StartAt = startAt,
            EndAt = endAt,
            IsAllDay = isAllDay,
            RecurrenceRule = recurrenceRule,
            Status = EventStatus.Upcoming
        };
    }

    public void Update(
        string title,
        string? description,
        string? location,
        DateTimeOffset startAt,
        DateTimeOffset? endAt,
        bool isAllDay,
        string? recurrenceRule)
    {
        if (endAt.HasValue && endAt.Value <= startAt)
            throw new ArgumentException("EndAt must be after StartAt.");

        Title = title;
        Description = description;
        Location = location;
        StartAt = startAt;
        EndAt = endAt;
        IsAllDay = isAllDay;
        RecurrenceRule = recurrenceRule;
    }
}
