using Shared.Infrastructure.Common;

namespace Chat.Domain.Entities;

public class ChatAccountLink : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public string StoatUserId { get; private set; } = string.Empty;
    public string StoatUsername { get; private set; } = string.Empty;
    public string StoatEmail { get; private set; } = string.Empty;
    public string EncryptedStoatPassword { get; private set; } = string.Empty;

    private ChatAccountLink() { }

    public static ChatAccountLink Create(
        Guid profileId,
        string stoatUserId,
        string stoatUsername,
        string stoatEmail,
        string encryptedStoatPassword)
    {
        return new ChatAccountLink
        {
            ProfileId = profileId,
            StoatUserId = stoatUserId,
            StoatUsername = stoatUsername,
            StoatEmail = stoatEmail,
            EncryptedStoatPassword = encryptedStoatPassword
        };
    }
}
