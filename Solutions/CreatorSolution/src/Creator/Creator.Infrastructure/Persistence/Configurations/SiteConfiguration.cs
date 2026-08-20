using Creator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Creator.Infrastructure.Persistence.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.OwnerProfileId).IsUnique();

        builder.Property(e => e.PublishState)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
