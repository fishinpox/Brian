using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;
using YouTube.Application.Common.Interfaces;

namespace YouTube.Application.Features.YouTube.Queries.GetYouTubeStatus;

public class GetYouTubeStatusQueryHandler(
    IYouTubeDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetYouTubeStatusQuery, Result<YouTubeStatusDto>>
{
    public async Task<Result<YouTubeStatusDto>> Handle(GetYouTubeStatusQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var hasCredential = await db.ExternalCredentials
            .AnyAsync(c => c.ProfileId == profileId, cancellationToken);

        var followedCount = await db.FollowedChannels
            .CountAsync(c => c.ProfileId == profileId, cancellationToken);

        return Result<YouTubeStatusDto>.Success(new YouTubeStatusDto(hasCredential, followedCount));
    }
}
