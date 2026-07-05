using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Persistence.Configurations;

public class StreamEventConfiguration : IEntityTypeConfiguration<StreamEvent>
{
    public void Configure(EntityTypeBuilder<StreamEvent> builder)
    {
        builder.ToTable("StreamEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ThumbnailUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.PlatformVideoId)
            .HasMaxLength(200);

        builder.Property(e => e.VodUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.Platform)
            .HasConversion<string>();

        builder.Property(e => e.Status)
            .HasConversion<string>();

        builder.HasIndex(e => e.PlatformVideoId)
            .IsUnique()
            .HasFilter("[PlatformVideoId] IS NOT NULL");

        builder.HasIndex(e => e.ScheduledStart);
    }
}
