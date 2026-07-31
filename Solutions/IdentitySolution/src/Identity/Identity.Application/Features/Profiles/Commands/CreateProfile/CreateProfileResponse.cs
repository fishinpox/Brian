using Identity.Application.Common.DTOs;

namespace Identity.Application.Features.Profiles.Commands.CreateProfile;

public record CreateProfileResponse(ProfileDto Profile, string Token, string RefreshToken);
