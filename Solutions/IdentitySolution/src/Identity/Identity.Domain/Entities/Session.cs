using Shared.Infrastructure.Common;

namespace Identity.Domain.Entities;

public class Session : BaseEntity
{
    public Guid AccountId { get; private set; }
    public Guid ProfileId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public bool IsRevoked { get; private set; }
    public Guid? ReplacedBySessionId { get; private set; }

    private Session() { }

    public static Session Issue(
        Guid accountId,
        Guid profileId,
        string tokenHash,
        DateTimeOffset expiresAt,
        string? ipAddress,
        string? userAgent)
    {
        return new Session
        {
            AccountId = accountId,
            ProfileId = profileId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsRevoked = false
        };
    }

    public void Revoke() => IsRevoked = true;

    /// <summary>Revokes this session and links it to the session that replaced it, for rotation-chain tracking.</summary>
    public void RotateTo(Guid newSessionId)
    {
        IsRevoked = true;
        ReplacedBySessionId = newSessionId;
    }
}
