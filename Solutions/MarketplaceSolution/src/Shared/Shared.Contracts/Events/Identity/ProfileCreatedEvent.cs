namespace Shared.Contracts.Events.Identity;

public record ProfileCreatedEvent(
    Guid ProfileId,
    Guid AccountId,
    string Username,
    string[] Roles,
    DateTimeOffset OccurredAt);
