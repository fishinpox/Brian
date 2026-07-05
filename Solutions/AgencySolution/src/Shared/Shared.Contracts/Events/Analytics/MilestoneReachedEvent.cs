namespace Shared.Contracts.Events.Analytics;

public record MilestoneReachedEvent(
    Guid MilestoneId,
    Guid VTuberProfileId,
    string MilestoneType,
    long TargetValue,
    string Platform,
    DateTimeOffset OccurredAt);
