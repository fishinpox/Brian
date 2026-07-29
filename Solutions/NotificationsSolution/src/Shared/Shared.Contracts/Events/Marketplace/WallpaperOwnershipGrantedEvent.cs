namespace Shared.Contracts.Events.Marketplace;

public record WallpaperOwnershipGrantedEvent(
    Guid OwnershipId,
    Guid ProfileId,
    Guid ItemId,
    DateTimeOffset OccurredAt);
