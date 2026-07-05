using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    Guid? PreferredProfileId) : IRequest<Result<LoginResponse>>;
