using Shared.Infrastructure.Common;

namespace Identity.Domain.Entities;

public class Session : BaseEntity
{
    public Guid AccountId { get; set; }
    public Guid ProfileId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsRevoked { get; set; } = false;
}
