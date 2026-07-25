using MediatR;
using Shared.Infrastructure.Common;

namespace YouTube.Application.Features.YouTube.Queries.GetYouTubeStatus;

public record YouTubeStatusDto(bool HasCredential, int FollowedCount);

public record GetYouTubeStatusQuery : IRequest<Result<YouTubeStatusDto>>;
