namespace Shared.Contracts.Events.Moderation;

public record CalendarBackgroundApprovedEvent(
    Guid SubmissionId,
    Guid ProfileId,
    DateTimeOffset OccurredAt);
