using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.Refresh;

public record RefreshTokenCommand(
    string RefreshToken,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<Result<RefreshTokenResponse>>;
