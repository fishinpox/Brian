using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

public class WallpaperOwnershipConfiguration : IEntityTypeConfiguration<WallpaperOwnership>
{
    public void Configure(EntityTypeBuilder<WallpaperOwnership> builder)
    {
        builder.ToTable("WallpaperOwnerships");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.ProfileId, e.ItemId }).IsUnique();
        builder.HasIndex(e => new { e.ProfileId, e.AppliedAt });
    }
}
