using MediatR;
using Shared.Infrastructure.Common;

namespace Holodex.Application.Features.Holodex.Queries.GetFollowedChannels;

public record GetFollowedChannelsQuery : IRequest<Result<List<HolodexFavoriteDto>>>;
