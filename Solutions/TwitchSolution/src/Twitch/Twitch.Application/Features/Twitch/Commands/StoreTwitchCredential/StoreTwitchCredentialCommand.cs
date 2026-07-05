using MediatR;
using Shared.Infrastructure.Common;

namespace Twitch.Application.Features.Twitch.Commands.StoreTwitchCredential;

public record StoreTwitchCredentialCommand(string ApiKey) : IRequest<Result>;
