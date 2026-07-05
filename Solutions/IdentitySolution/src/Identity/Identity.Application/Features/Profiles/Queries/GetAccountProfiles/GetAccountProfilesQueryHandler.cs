using Identity.Application.Common.DTOs;
using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Profiles.Queries.GetAccountProfiles;

public class GetAccountProfilesQueryHandler(
    IIdentityDbContext db,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetAccountProfilesQuery, Result<List<ProfileDto>>>
{
    public async Task<Result<List<ProfileDto>>> Handle(GetAccountProfilesQuery request, CancellationToken cancellationToken)
    {
        var accountId = currentUserService.AccountId;
        if (accountId is null)
            return Result<List<ProfileDto>>.Failure("User is not authenticated.");

        var profiles = await db.Profiles
            .Include(p => p.Roles)
            .Where(p => p.AccountId == accountId.Value)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = profiles.Select(p => new ProfileDto(
            p.Id,
            p.AccountId,
            p.Username,
            p.DisplayName,
            p.AvatarUrl,
            p.Bio,
            p.IsPublic,
            p.Roles.Select(r => r.Role.ToString()).ToArray(),
            p.CreatedAt)).ToList();

        return Result<List<ProfileDto>>.Success(dtos);
    }
}
