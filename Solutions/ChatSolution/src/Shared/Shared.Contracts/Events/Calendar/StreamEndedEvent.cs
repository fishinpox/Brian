namespace Shared.Contracts.Events.Calendar;

public record StreamEndedEvent(
    Guid StreamEventId,
    Guid VTuberProfileId,
    string Platform,
    DateTimeOffset EndedAt,
    int? PeakViewers,
    DateTimeOffset OccurredAt);
