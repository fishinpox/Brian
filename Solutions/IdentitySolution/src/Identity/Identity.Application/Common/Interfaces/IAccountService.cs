namespace Identity.Application.Common.Interfaces;

public interface IAccountService
{
    Task<(bool Success, Guid AccountId, string[] Errors)> CreateAccountAsync(string email, string password, CancellationToken ct);
    Task<(bool Success, Guid AccountId, string[] Errors)> ValidateCredentialsAsync(string email, string password, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);

    /// Sets (or replaces) the account's password. Used to let Google-only
    /// accounts pick a real password during profile setup -- new Google
    /// accounts are created with a random password the user never sees
    /// (see GoogleLoginCommandHandler.HandleNewUserAsync), so without this
    /// they'd have no way to log in if they lost access to Google.
    Task<(bool Success, string[] Errors)> SetPasswordAsync(Guid accountId, string newPassword, CancellationToken ct);
}
