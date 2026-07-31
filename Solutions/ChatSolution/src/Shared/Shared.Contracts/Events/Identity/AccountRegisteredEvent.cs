namespace Shared.Contracts.Events.Identity;

public record AccountRegisteredEvent(
    Guid AccountId,
    string Email,
    DateTimeOffset OccurredAt);
