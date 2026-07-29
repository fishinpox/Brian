namespace Marketplace.Application.Features.Purchases.Commands.PurchaseItem;

public record PurchaseItemResponse(Guid OwnershipId, Guid ItemId, DateTimeOffset PurchasedAt);
