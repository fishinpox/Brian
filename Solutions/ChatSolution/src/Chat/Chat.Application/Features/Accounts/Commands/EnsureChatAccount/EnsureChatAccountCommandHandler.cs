using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Chat.Application.Common.Interfaces;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Chat.Application.Features.Accounts.Commands.EnsureChatAccount;

/// <summary>
/// Idempotent "does this profile have a linked, fully-provisioned Stoat account; if not, create
/// one" - the account-creation/login/onboarding sequence here is exactly what the ChatWindow spike
/// confirmed works end-to-end against a real self-hosted instance with no manual admin step.
/// </summary>
public partial class EnsureChatAccountCommandHandler(
    IChatDbContext db,
    ICurrentUserService currentUserService,
    IStoatApiClient stoatClient,
    ICredentialEncryptionService encryptionService)
    : IRequestHandler<EnsureChatAccountCommand, Result<EnsureChatAccountResponse>>
{
    public async Task<Result<EnsureChatAccountResponse>> Handle(EnsureChatAccountCommand request, CancellationToken cancellationToken)
    {
        var profileId = currentUserService.ProfileId;
        if (profileId is null)
            return Result<EnsureChatAccountResponse>.Failure("User is not authenticated.");

        var existing = await db.ChatAccountLinks
            .FirstOrDefaultAsync(l => l.ProfileId == profileId.Value, cancellationToken);

        if (existing is not null)
        {
            var password = encryptionService.Decrypt(existing.EncryptedStoatPassword);
            return Result<EnsureChatAccountResponse>.Success(
                new EnsureChatAccountResponse(existing.StoatUsername, existing.StoatEmail, password));
        }

        var email = $"chat-{profileId.Value:N}@chat.internal.brian";
        var generatedPassword = GeneratePassword();
        var desiredUsername = SanitizeUsername(currentUserService.Username);

        await stoatClient.CreateAccountAsync(new StoatCreateAccountRequest(email, generatedPassword));

        var login = await stoatClient.LoginAsync(new StoatLoginRequest(email, generatedPassword, "Brian Chat"));

        var onboard = await stoatClient.CompleteOnboardingAsync(
            login.Token, new StoatOnboardCompleteRequest(desiredUsername));

        var link = ChatAccountLink.Create(
            profileId.Value,
            login.UserId,
            onboard.Username,
            email,
            encryptionService.Encrypt(generatedPassword));

        db.ChatAccountLinks.Add(link);
        await db.SaveChangesAsync(cancellationToken);

        return Result<EnsureChatAccountResponse>.Success(
            new EnsureChatAccountResponse(onboard.Username, email, generatedPassword));
    }

    private static string GeneratePassword() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));

    // Stoat auto-assigns a discriminator on collision (confirmed during the spike), so this only
    // needs to satisfy its character-set/length constraints, not guarantee uniqueness itself.
    private static string SanitizeUsername(string? username)
    {
        var cleaned = UsernameCharsRegex().Replace(username ?? string.Empty, "_");
        if (cleaned.Length > 32) cleaned = cleaned[..32];
        return cleaned.Length >= 2 ? cleaned : "BrianUser";
    }

    [GeneratedRegex("[^a-zA-Z0-9_]")]
    private static partial Regex UsernameCharsRegex();
}
