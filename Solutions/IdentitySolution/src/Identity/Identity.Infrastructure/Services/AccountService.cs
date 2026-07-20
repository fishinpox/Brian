using Identity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services;

public class AccountService(UserManager<ApplicationUser> userManager) : IAccountService
{
    public async Task<(bool Success, Guid AccountId, string[] Errors)> CreateAccountAsync(string email, string password, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, Guid.Empty, result.Errors.Select(e => e.Description).ToArray());

        return (true, user.Id, []);
    }

    public async Task<(bool Success, Guid AccountId, string[] Errors)> ValidateCredentialsAsync(string email, string password, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, Guid.Empty, ["Invalid credentials."]);

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid)
            return (false, Guid.Empty, ["Invalid credentials."]);

        return (true, user.Id, []);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<(bool Success, string[] Errors)> SetPasswordAsync(Guid accountId, string newPassword, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(accountId.ToString());
        if (user is null)
            return (false, ["Account not found."]);

        if (await userManager.HasPasswordAsync(user))
        {
            var removeResult = await userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                return (false, removeResult.Errors.Select(e => e.Description).ToArray());
        }

        var addResult = await userManager.AddPasswordAsync(user, newPassword);
        if (!addResult.Succeeded)
            return (false, addResult.Errors.Select(e => e.Description).ToArray());

        return (true, []);
    }
}
