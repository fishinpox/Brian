namespace Shared.Contracts.Events.Marketplace;

public record OrderPlacedEvent(
    Guid OrderId,
    Guid BuyerProfileId,
    Guid SellerProfileId,
    Guid ListingId,
    decimal AmountUsd,
    DateTimeOffset OccurredAt);
