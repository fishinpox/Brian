namespace Shared.Contracts.Events.Calendar;

public record StreamGoLiveEvent(
    Guid StreamEventId,
    Guid VTuberProfileId,
    string Title,
    string Platform,
    string? ThumbnailUrl,
    DateTimeOffset StartedAt,
    DateTimeOffset OccurredAt);
