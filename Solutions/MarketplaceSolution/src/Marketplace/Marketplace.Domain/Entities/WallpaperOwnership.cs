using Shared.Infrastructure.Common;

namespace Marketplace.Domain.Entities;

public class WallpaperOwnership : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public Guid ItemId { get; private set; }
    public DateTimeOffset PurchasedAt { get; private set; }

    /// <summary>
    /// Null means the purchase hasn't been applied to a Windows desktop yet - this is the
    /// "pending apply" signal the native app polls for. NOTE: this is per-ownership-row,
    /// not per-device - if the same profile is signed into the native app on two machines,
    /// whichever applies first clears this for both. Acceptable for the single-device test
    /// workflow this supports; a real multi-device story would need a per-device join table.
    /// </summary>
    public DateTimeOffset? AppliedAt { get; private set; }

    private WallpaperOwnership() { }

    public static WallpaperOwnership Purchase(Guid profileId, Guid itemId)
    {
        return new WallpaperOwnership
        {
            ProfileId = profileId,
            ItemId = itemId,
            PurchasedAt = DateTimeOffset.UtcNow,
            AppliedAt = null
        };
    }

    public void MarkApplied() => AppliedAt = DateTimeOffset.UtcNow;
}
