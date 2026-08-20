using Creator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Creator.Infrastructure.Persistence.Configurations;

public class SiteAccessGrantConfiguration : IEntityTypeConfiguration<SiteAccessGrant>
{
    public void Configure(EntityTypeBuilder<SiteAccessGrant> builder)
    {
        builder.ToTable("SiteAccessGrants");

        builder.HasKey(e => e.Id);

        // Filtered so a revoked-then-re-granted profile doesn't collide with its own history.
        builder.HasIndex(e => new { e.SiteId, e.ProfileId })
            .IsUnique()
            .HasFilter("[RevokedAt] IS NULL");

        builder.Property(e => e.AccessLevel)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
