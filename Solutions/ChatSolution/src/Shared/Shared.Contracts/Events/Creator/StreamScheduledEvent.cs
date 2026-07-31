namespace Shared.Contracts.Events.Creator;

public record StreamScheduledEvent(
    Guid StreamEventId,
    Guid VTuberProfileId,
    string Title,
    string Platform,
    DateTimeOffset ScheduledStart,
    DateTimeOffset OccurredAt);
