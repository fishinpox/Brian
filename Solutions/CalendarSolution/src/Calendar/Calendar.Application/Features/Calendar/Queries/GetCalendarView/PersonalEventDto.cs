namespace Calendar.Application.Features.Calendar.Queries.GetCalendarView;

public record PersonalEventDto(
    Guid Id,
    string Title,
    string? Description,
    string? Location,
    DateTimeOffset StartAt,
    DateTimeOffset? EndAt,
    bool IsAllDay,
    string Status,
    Guid? SubfolderId,
    bool IsVisible,
    bool IsDraggable,
    bool IsCompleted,
    string? CountdownCategory,
    string RecurrenceType,
    DateTimeOffset? RecurrenceEndDate,
    bool AutoDeferEnabled);
