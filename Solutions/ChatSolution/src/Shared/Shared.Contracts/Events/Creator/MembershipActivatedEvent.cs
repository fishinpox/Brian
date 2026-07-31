namespace Shared.Contracts.Events.Creator;

public record MembershipActivatedEvent(
    Guid FanMembershipId,
    Guid FanProfileId,
    Guid TierId,
    Guid VTuberProfileId,
    string TierName,
    DateTimeOffset OccurredAt);
