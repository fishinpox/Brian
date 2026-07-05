using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Profiles.Commands.SwitchActiveProfile;

public class SwitchActiveProfileCommandHandler(
    IIdentityDbContext db,
    ICurrentUserService currentUserService,
    ITokenService tokenService)
    : IRequestHandler<SwitchActiveProfileCommand, Result<SwitchProfileResponse>>
{
    public async Task<Result<SwitchProfileResponse>> Handle(SwitchActiveProfileCommand request, CancellationToken cancellationToken)
    {
        var accountId = currentUserService.AccountId;
        if (accountId is null)
            return Result<SwitchProfileResponse>.Failure("User is not authenticated.");

        var profile = await db.Profiles
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == request.ProfileId && p.AccountId == accountId.Value, cancellationToken);

        if (profile is null)
            return Result<SwitchProfileResponse>.Failure("Profile not found or does not belong to this account.");

        var token = tokenService.GenerateToken(accountId.Value, profile);
        var roles = profile.Roles.Select(r => r.Role.ToString()).ToArray();

        return Result<SwitchProfileResponse>.Success(
            new SwitchProfileResponse(profile.Id, token, roles));
    }
}
