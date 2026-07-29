using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    Guid? PreferredProfileId,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<Result<LoginResponse>>;
