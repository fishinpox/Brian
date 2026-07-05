using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class ProfileRoleConfiguration : IEntityTypeConfiguration<ProfileRole>
{
    public void Configure(EntityTypeBuilder<ProfileRole> builder)
    {
        builder.ToTable("ProfileRoles");

        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.ProfileId, r.Role });
    }
}
