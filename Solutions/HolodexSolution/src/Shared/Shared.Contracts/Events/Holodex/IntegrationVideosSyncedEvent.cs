namespace Shared.Contracts.Events.Holodex;

public record ExternalVideoDto(
    string ExternalVideoId,
    string Title,
    string? ThumbnailUrl,
    string Status,
    DateTimeOffset? ScheduledStart,
    DateTimeOffset? ActualStart,
    DateTimeOffset? ActualEnd);

public record IntegrationVideosSyncedEvent(
    Guid ProfileId,
    string Provider,
    List<ExternalVideoDto> Videos,
    DateTimeOffset OccurredAt);
