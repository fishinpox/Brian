namespace Identity.Application.Features.Auth.Commands.RegisterAccount;

public record RegisterAccountResponse(Guid AccountId, Guid ProfileId, string Token);
