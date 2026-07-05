namespace Shared.Contracts.Events.Creator;

public record FanArtApprovedEvent(
    Guid SubmissionId,
    Guid SubmitterProfileId,
    Guid VTuberProfileId,
    string ImageUrl,
    DateTimeOffset OccurredAt);
