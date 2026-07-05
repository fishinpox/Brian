using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class OAuthIdentityConfiguration : IEntityTypeConfiguration<OAuthIdentity>
{
    public void Configure(EntityTypeBuilder<OAuthIdentity> builder)
    {
        builder.ToTable("OAuthIdentities");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.ProviderUserId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.EncryptedAccessToken)
            .HasMaxLength(2000);

        builder.Property(o => o.EncryptedRefreshToken)
            .HasMaxLength(2000);

        builder.HasIndex(o => new { o.AccountId, o.Provider, o.ProviderUserId });
    }
}
