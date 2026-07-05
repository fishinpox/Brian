using Identity.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Identity.Domain.Entities;

public class ProfileRole : BaseEntity
{
    public Guid ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
}
