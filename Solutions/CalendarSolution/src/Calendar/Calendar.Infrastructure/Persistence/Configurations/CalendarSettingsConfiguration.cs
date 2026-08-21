using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Persistence.Configurations;

public class CalendarSettingsConfiguration : IEntityTypeConfiguration<CalendarSettings>
{
    public void Configure(EntityTypeBuilder<CalendarSettings> builder)
    {
        builder.ToTable("CalendarSettings");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.ProfileId).IsUnique();
    }
}
