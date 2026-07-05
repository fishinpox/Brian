using MediatR;
using Shared.Infrastructure.Common;

namespace YouTube.Application.Features.YouTube.Commands.StoreYouTubeCredential;

public record StoreYouTubeCredentialCommand(string ApiKey) : IRequest<Result>;
