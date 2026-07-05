namespace Identity.Application.Features.Profiles.Commands.SwitchActiveProfile;

public record SwitchProfileResponse(Guid ProfileId, string Token, string[] Roles);
