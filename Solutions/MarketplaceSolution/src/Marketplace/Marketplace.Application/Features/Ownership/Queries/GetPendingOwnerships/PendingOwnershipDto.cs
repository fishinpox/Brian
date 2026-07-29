namespace Marketplace.Application.Features.Ownership.Queries.GetPendingOwnerships;

public record PendingOwnershipDto(Guid OwnershipId, Guid ItemId, string ItemName, DateTimeOffset PurchasedAt);
