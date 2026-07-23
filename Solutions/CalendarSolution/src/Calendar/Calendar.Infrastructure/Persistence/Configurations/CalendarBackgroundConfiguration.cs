using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Persistence.Configurations;

public class CalendarBackgroundConfiguration : IEntityTypeConfiguration<CalendarBackground>
{
    public void Configure(EntityTypeBuilder<CalendarBackground> builder)
    {
        builder.ToTable("CalendarBackgrounds");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ImageData)
            .IsRequired()
            .HasColumnType("varbinary(max)");

        builder.Property(e => e.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(e => e.Status)
            .HasConversion<string>();

        builder.HasIndex(e => e.ProfileId).IsUnique();
    }
}
