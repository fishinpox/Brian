namespace Shared.Contracts.Events.Marketplace;

public record OrderDeliveredEvent(
    Guid OrderId,
    Guid BuyerProfileId,
    Guid SellerProfileId,
    string? DeliveryUrl,
    DateTimeOffset OccurredAt);
