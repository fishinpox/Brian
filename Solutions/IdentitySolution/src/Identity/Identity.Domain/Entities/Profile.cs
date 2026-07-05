using Shared.Infrastructure.Common;

namespace Identity.Domain.Entities;

public class Profile : BaseAuditableEntity
{
    public Guid AccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsPublic { get; set; } = true;
    public ICollection<ProfileRole> Roles { get; set; } = [];
}
