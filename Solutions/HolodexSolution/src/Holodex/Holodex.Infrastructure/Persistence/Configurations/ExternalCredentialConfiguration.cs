using Holodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holodex.Infrastructure.Persistence.Configurations;

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
