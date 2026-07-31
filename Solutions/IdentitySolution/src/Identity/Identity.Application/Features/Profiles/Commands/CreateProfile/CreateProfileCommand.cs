using Identity.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Profiles.Commands.CreateProfile;

public record CreateProfileCommand(
    string Username,
    string DisplayName,
    UserRole Role,
    string? AvatarUrl,
    string Password,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<Result<CreateProfileResponse>>;
