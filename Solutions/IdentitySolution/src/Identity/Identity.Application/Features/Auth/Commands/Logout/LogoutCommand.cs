using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;
