using Holodex.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holodex.Infrastructure.Persistence.Configurations;

public class FollowedChannelConfiguration : IEntityTypeConfiguration<FollowedChannel>
{
    public void Configure(EntityTypeBuilder<FollowedChannel> builder)
    {
        builder.ToTable("FollowedChannels");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.HolodexChannelId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.EnglishName)
            .HasMaxLength(200);

        builder.HasIndex(c => new { c.ProfileId, c.HolodexChannelId })
            .IsUnique();
    }
}
