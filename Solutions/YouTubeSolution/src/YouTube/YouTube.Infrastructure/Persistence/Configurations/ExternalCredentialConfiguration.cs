using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTube.Domain.Entities;

namespace YouTube.Infrastructure.Persistence.Configurations;

public class ExternalCredentialConfiguration : IEntityTypeConfiguration<ExternalCredential>
{
    public void Configure(EntityTypeBuilder<ExternalCredential> builder)
    {
        builder.ToTable("ExternalCredentials");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.EncryptedValue)
            .IsRequired();

        builder.HasIndex(c => c.ProfileId)
            .IsUnique();
    }
}
