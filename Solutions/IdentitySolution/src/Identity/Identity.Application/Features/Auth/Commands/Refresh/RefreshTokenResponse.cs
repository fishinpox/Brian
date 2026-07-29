namespace Identity.Application.Features.Auth.Commands.Refresh;

public record RefreshTokenResponse(Guid AccountId, Guid ProfileId, string Token, string RefreshToken, string[] Roles);
