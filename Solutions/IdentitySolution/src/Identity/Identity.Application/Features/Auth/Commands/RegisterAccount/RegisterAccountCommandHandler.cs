using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.RegisterAccount;

public class RegisterAccountCommandHandler(
    IAccountService accountService,
    IIdentityDbContext db,
    ITokenService tokenService)
    : IRequestHandler<RegisterAccountCommand, Result<RegisterAccountResponse>>
{
    public async Task<Result<RegisterAccountResponse>> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        if (await accountService.EmailExistsAsync(request.Email, cancellationToken))
            return Result<RegisterAccountResponse>.Failure("Email is already in use.");

        var usernameExists = await db.Profiles
            .AnyAsync(p => p.Username == request.Username, cancellationToken);
        if (usernameExists)
            return Result<RegisterAccountResponse>.Failure("Username is already taken.");

        var (success, accountId, errors) = await accountService.CreateAccountAsync(request.Email, request.Password, cancellationToken);
        if (!success)
            return Result<RegisterAccountResponse>.Failure(errors);

        var profile = new Profile
        {
            AccountId = accountId,
            Username = request.Username,
            DisplayName = request.DisplayName,
            IsPublic = true
        };

        var profileRole = new ProfileRole
        {
            ProfileId = profile.Id,
            Profile = profile,
            Role = UserRole.Fan,
            GrantedAt = DateTimeOffset.UtcNow
        };

        profile.Roles.Add(profileRole);

        db.Profiles.Add(profile);
        await db.SaveChangesAsync(cancellationToken);

        var token = tokenService.GenerateToken(accountId, profile);

        return Result<RegisterAccountResponse>.Success(
            new RegisterAccountResponse(accountId, profile.Id, token));
    }
}
