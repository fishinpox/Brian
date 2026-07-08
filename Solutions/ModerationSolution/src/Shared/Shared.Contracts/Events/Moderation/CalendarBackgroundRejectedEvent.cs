namespace Shared.Contracts.Events.Moderation;

public record CalendarBackgroundRejectedEvent(
    Guid SubmissionId,
    Guid ProfileId,
    string Reason,
    DateTimeOffset OccurredAt);
