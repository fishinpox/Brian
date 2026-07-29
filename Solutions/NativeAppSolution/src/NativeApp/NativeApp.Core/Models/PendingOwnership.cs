namespace NativeApp.Core.Models;

public record PendingOwnership(Guid OwnershipId, Guid ItemId, string ItemName, DateTimeOffset PurchasedAt);
