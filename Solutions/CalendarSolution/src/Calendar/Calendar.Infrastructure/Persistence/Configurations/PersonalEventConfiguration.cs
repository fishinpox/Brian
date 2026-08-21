using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Persistence.Configurations;

public class PersonalEventConfiguration : IEntityTypeConfiguration<PersonalEvent>
{
    public void Configure(EntityTypeBuilder<PersonalEvent> builder)
    {
        builder.ToTable("PersonalEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Location)
            .HasMaxLength(500);

        builder.Property(e => e.RecurrenceRule)
            .HasMaxLength(1000);

        builder.Property(e => e.Status)
            .HasConversion<string>();

        builder.Property(e => e.IsVisible)
            .HasDefaultValue(true);

        builder.Property(e => e.CountdownCategory)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.RecurrenceType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.RecurrenceType.None);

        // AutoDeferEnabled defaults to true in the C# entity, but EF's migration generator only
        // picks that up from this explicit HasDefaultValue - a property initializer alone silently
        // backfills existing rows to false (caught the hard way on IsVisible in the Folders slice).
        builder.Property(e => e.AutoDeferEnabled)
            .HasDefaultValue(true);

        builder.HasIndex(e => new { e.ProfileId, e.StartAt, e.EndAt });
        builder.HasIndex(e => e.SubfolderId);

        builder.HasOne<Subfolder>()
            .WithMany()
            .HasForeignKey(e => e.SubfolderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
