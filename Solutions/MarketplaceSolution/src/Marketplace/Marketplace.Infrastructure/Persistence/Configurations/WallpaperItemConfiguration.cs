using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

public class WallpaperItemConfiguration : IEntityTypeConfiguration<WallpaperItem>
{
    public void Configure(EntityTypeBuilder<WallpaperItem> builder)
    {
        builder.ToTable("WallpaperItems");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.ImageData)
            .IsRequired()
            .HasColumnType("varbinary(max)");

        builder.Property(e => e.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(e => e.Price)
            .HasColumnType("decimal(10,2)");
    }
}
