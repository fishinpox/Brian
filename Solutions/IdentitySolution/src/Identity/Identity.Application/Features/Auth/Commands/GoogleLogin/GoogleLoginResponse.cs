namespace Identity.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginResponse(Guid AccountId, Guid? ProfileId, string Token, bool NeedsProfile);
