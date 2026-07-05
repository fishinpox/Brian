using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.RegisterAccount;

public record RegisterAccountCommand(
    string Email,
    string Password,
    string Username,
    string DisplayName) : IRequest<Result<RegisterAccountResponse>>;
