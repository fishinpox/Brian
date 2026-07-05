using Identity.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Identity.Domain.Entities;

public class OAuthIdentity : BaseEntity
{
    public Guid AccountId { get; set; }
    public Platform Provider { get; set; }
    public string ProviderUserId { get; set; } = string.Empty;
    public string? EncryptedAccessToken { get; set; }
    public string? EncryptedRefreshToken { get; set; }
    public DateTimeOffset? TokenExpiresAt { get; set; }
}
