using Shared.Infrastructure.Common;

namespace Twitch.Domain.Entities;

public class ExternalCredential : BaseEntity
{
    public Guid ProfileId { get; private set; }
    public string EncryptedValue { get; set; } = string.Empty;
    public DateTimeOffset? ExpiresAt { get; set; }

    private ExternalCredential() { }

    public static ExternalCredential Create(
        Guid profileId,
        string encryptedValue,
        DateTimeOffset? expiresAt = null)
    {
        return new ExternalCredential
        {
            ProfileId = profileId,
            EncryptedValue = encryptedValue,
            ExpiresAt = expiresAt
        };
    }
}
