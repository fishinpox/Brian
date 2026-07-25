using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;
using YouTube.Application.Common.Interfaces;

namespace YouTube.Application.Features.YouTube.Queries.GetFollowedChannels;

public class GetFollowedChannelsQueryHandler(
    IYouTubeDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetFollowedChannelsQuery, Result<List<YouTubeFavoriteDto>>>
{
    public async Task<Result<List<YouTubeFavoriteDto>>> Handle(GetFollowedChannelsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var followed = await db.FollowedChannels
            .Where(c => c.ProfileId == profileId)
            .Select(c => new YouTubeFavoriteDto(c.YouTubeChannelId, c.Name, c.EnglishName, c.PhotoUrl, true))
            .ToListAsync(cancellationToken);

        return Result<List<YouTubeFavoriteDto>>.Success(followed);
    }
}
