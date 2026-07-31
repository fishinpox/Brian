using Chat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Persistence.Configurations;

public class ChatAccountLinkConfiguration : IEntityTypeConfiguration<ChatAccountLink>
{
    public void Configure(EntityTypeBuilder<ChatAccountLink> builder)
    {
        builder.ToTable("ChatAccountLinks");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.ProfileId).IsUnique();

        builder.Property(e => e.StoatUserId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.StoatUsername)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.StoatEmail)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(e => e.EncryptedStoatPassword)
            .IsRequired()
            .HasMaxLength(500);
    }
}
