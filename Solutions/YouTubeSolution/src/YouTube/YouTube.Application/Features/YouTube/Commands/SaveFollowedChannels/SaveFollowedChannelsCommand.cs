using MediatR;
using Shared.Infrastructure.Common;

namespace YouTube.Application.Features.YouTube.Commands.SaveFollowedChannels;

public record SaveFollowedChannelsCommand(List<YouTubeFavoriteDto> Channels) : IRequest<Result<int>>;
