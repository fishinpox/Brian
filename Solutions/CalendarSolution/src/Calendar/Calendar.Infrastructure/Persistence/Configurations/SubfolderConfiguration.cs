using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Persistence.Configurations;

public class SubfolderConfiguration : IEntityTypeConfiguration<Subfolder>
{
    public void Configure(EntityTypeBuilder<Subfolder> builder)
    {
        builder.ToTable("Subfolders");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.FolderId);
        builder.HasIndex(e => e.ProfileId);
    }
}
