namespace Identity.Application.Features.Auth.Commands.Login;

public record LoginResponse(Guid AccountId, Guid ProfileId, string Token, string[] Roles);
