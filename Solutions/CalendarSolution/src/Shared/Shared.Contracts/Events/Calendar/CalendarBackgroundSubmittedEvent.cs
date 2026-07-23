namespace Shared.Contracts.Events.Calendar;

public record CalendarBackgroundSubmittedEvent(
    Guid SubmissionId,
    Guid ProfileId,
    byte[] ImageData,
    string ContentType,
    string FileName,
    long SizeBytes,
    DateTimeOffset OccurredAt);
