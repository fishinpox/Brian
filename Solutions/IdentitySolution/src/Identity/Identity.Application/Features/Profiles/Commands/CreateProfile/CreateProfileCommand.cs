using Identity.Application.Common.DTOs;
using Identity.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Profiles.Commands.CreateProfile;

public record CreateProfileCommand(
    string Username,
    string DisplayName,
    UserRole Role,
    string? AvatarUrl) : IRequest<Result<ProfileDto>>;
