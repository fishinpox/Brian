namespace Shared.Contracts.Events.Creator;

public record MembershipCancelledEvent(
    Guid FanMembershipId,
    Guid FanProfileId,
    Guid VTuberProfileId,
    DateTimeOffset OccurredAt);
