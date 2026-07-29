using Marketplace.Domain.Entities;
using Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Seed;

/// <summary>
/// Ensures at least one purchasable, pre-approved fixture wallpaper exists on a fresh environment
/// (Documentation/Calendar/CustomizeBackground.md's "at least one approved test item exists" precondition).
/// Not EF HasData: that requires a fixed literal Guid, which conflicts with entities getting their
/// Id from Guid.CreateVersion7() in the constructor per this repo's convention.
/// </summary>
public static class WallpaperCatalogSeeder
{
    // A minimal valid 1x1 PNG, embedded rather than a committed binary asset - this is a test
    // fixture, not real content, and keeps the seeder self-contained.
    private const string FixtureImageBase64 =
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";

    public static async Task SeedAsync(MarketplaceDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.WallpaperItems.AnyAsync(cancellationToken))
            return;

        var fixtureItem = WallpaperItem.Create(
            name: "Fixture Wallpaper",
            description: "Pre-approved test wallpaper for the purchase-to-desktop-apply verification workflow.",
            imageData: Convert.FromBase64String(FixtureImageBase64),
            contentType: "image/png",
            fileName: "fixture-wallpaper.png",
            price: 0m);

        db.WallpaperItems.Add(fixtureItem);
        await db.SaveChangesAsync(cancellationToken);
    }
}
