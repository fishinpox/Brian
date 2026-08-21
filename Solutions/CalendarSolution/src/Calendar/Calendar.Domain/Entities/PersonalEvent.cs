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
    public Guid? SubfolderId { get; private set; }
    public bool IsVisible { get; private set; } = true;
    public CountdownCategory? CountdownCategory { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public RecurrenceType RecurrenceType { get; private set; } = RecurrenceType.None;
    public DateTimeOffset? RecurrenceEndDate { get; private set; }
    public bool AutoDeferEnabled { get; private set; } = true;
    public int DeferCount { get; private set; }

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

    public void Reschedule(DateTimeOffset newStartAt)
    {
        if (EndAt.HasValue)
        {
            var duration = EndAt.Value - StartAt;
            EndAt = newStartAt + duration;
        }

        StartAt = newStartAt;
    }

    public void AssignToSubfolder(Guid? subfolderId) => SubfolderId = subfolderId;

    public void SetVisibility(bool isVisible) => IsVisible = isVisible;

    public void SetCountdownCategory(CountdownCategory? category) => CountdownCategory = category;

    public void SetCompletion(bool isCompleted)
    {
        IsCompleted = isCompleted;
        CompletedAt = isCompleted ? DateTimeOffset.UtcNow : null;
    }

    public void SetRecurrence(RecurrenceType recurrenceType, DateTimeOffset? recurrenceEndDate)
    {
        RecurrenceType = recurrenceType;
        RecurrenceEndDate = recurrenceEndDate;
    }

    public void SetAutoDefer(bool autoDeferEnabled) => AutoDeferEnabled = autoDeferEnabled;

    /// <summary>
    /// Distinct from <see cref="Reschedule"/> (used by drag-and-drop) - this bumps DeferCount so the
    /// auto-defer job's 7-consecutive-day cap can actually stop deferring, per the story's requirement.
    /// </summary>
    public void DeferToDate(DateTimeOffset newStartAt)
    {
        Reschedule(newStartAt);
        DeferCount++;
    }
}
