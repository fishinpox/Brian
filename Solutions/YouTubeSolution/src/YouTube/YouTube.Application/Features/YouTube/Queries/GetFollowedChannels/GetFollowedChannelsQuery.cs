using MediatR;
using Shared.Infrastructure.Common;

namespace YouTube.Application.Features.YouTube.Queries.GetFollowedChannels;

public record GetFollowedChannelsQuery : IRequest<Result<List<YouTubeFavoriteDto>>>;
