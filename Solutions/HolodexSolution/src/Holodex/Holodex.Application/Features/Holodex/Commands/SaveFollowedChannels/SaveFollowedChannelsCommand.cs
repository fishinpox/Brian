using MediatR;
using Shared.Infrastructure.Common;

namespace Holodex.Application.Features.Holodex.Commands.SaveFollowedChannels;

public record SaveFollowedChannelsCommand(List<HolodexFavoriteDto> Channels) : IRequest<Result<int>>;
