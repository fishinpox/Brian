namespace Identity.Application.Common.Interfaces;

public interface IAccountService
{
    Task<(bool Success, Guid AccountId, string[] Errors)> CreateAccountAsync(string email, string password, CancellationToken ct);
    Task<(bool Success, Guid AccountId, string[] Errors)> ValidateCredentialsAsync(string email, string password, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
}
