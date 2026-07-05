using Identity.Application.Common.DTOs;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events.Identity;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Profiles.Commands.CreateProfile;

public class CreateProfileCommandHandler(
    IIdentityDbContext db,
    ICurrentUserService currentUserService,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<CreateProfileCommand, Result<ProfileDto>>
{
    public async Task<Result<ProfileDto>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var accountId = currentUserService.AccountId;
        if (accountId is null)
            return Result<ProfileDto>.Failure("User is not authenticated.");

        var usernameExists = await db.Profiles
            .AnyAsync(p => p.Username == request.Username, cancellationToken);
        if (usernameExists)
            return Result<ProfileDto>.Failure("Username is already taken.");

        var profile = new Profile
        {
            AccountId = accountId.Value,
            Username = request.Username,
            DisplayName = request.DisplayName,
            AvatarUrl = request.AvatarUrl,
            IsPublic = true
        };

        var profileRole = new ProfileRole
        {
            ProfileId = profile.Id,
            Profile = profile,
            Role = request.Role,
            GrantedAt = DateTimeOffset.UtcNow
        };

        profile.Roles.Add(profileRole);

        db.Profiles.Add(profile);
        await db.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new ProfileCreatedEvent(
            profile.Id,
            accountId.Value,
            profile.Username,
            profile.Roles.Select(r => r.Role.ToString()).ToArray(),
            DateTimeOffset.UtcNow), cancellationToken);

        var dto = new ProfileDto(
            profile.Id,
            profile.AccountId,
            profile.Username,
            profile.DisplayName,
            profile.AvatarUrl,
            profile.Bio,
            profile.IsPublic,
            profile.Roles.Select(r => r.Role.ToString()).ToArray(),
            profile.CreatedAt);

        return Result<ProfileDto>.Success(dto);
    }
}
