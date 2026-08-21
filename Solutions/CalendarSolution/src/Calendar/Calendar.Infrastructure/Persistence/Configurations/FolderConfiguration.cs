using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Persistence.Configurations;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.ToTable("Folders");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ColorBackground).IsRequired().HasMaxLength(7);
        builder.Property(e => e.ColorText).IsRequired().HasMaxLength(7);
        builder.Property(e => e.ColorBorder).IsRequired().HasMaxLength(7);

        builder.HasIndex(e => e.ProfileId);
    }
}
