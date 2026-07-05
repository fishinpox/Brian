using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IAccountService accountService,
    IIdentityDbContext db,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, accountId, errors) = await accountService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (!success)
            return Result<LoginResponse>.Failure(errors);

        var profiles = await db.Profiles
            .Include(p => p.Roles)
            .Where(p => p.AccountId == accountId)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        if (profiles.Count == 0)
            return Result<LoginResponse>.Failure("No profiles found for this account.");

        var profile = request.PreferredProfileId.HasValue
            ? profiles.FirstOrDefault(p => p.Id == request.PreferredProfileId.Value) ?? profiles[0]
            : profiles[0];

        var token = tokenService.GenerateToken(accountId, profile);
        var roles = profile.Roles.Select(r => r.Role.ToString()).ToArray();

        return Result<LoginResponse>.Success(
            new LoginResponse(accountId, profile.Id, token, roles));
    }
}
