using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string Code) : IRequest<Result<GoogleLoginResponse>>;
