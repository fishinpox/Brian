namespace Calendar.Application.Features.Calendar.Queries.GetCalendarView;

public record StreamEventDto(
    Guid Id,
    string Title,
    string? ThumbnailUrl,
    string Platform,
    DateTimeOffset ScheduledStart,
    string Status,
    bool IsLive);
