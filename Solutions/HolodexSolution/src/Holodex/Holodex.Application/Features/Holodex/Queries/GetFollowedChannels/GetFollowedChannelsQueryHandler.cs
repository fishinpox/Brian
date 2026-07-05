using Holodex.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Holodex.Application.Features.Holodex.Queries.GetFollowedChannels;

public class GetFollowedChannelsQueryHandler(
    IHolodexDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetFollowedChannelsQuery, Result<List<HolodexFavoriteDto>>>
{
    public async Task<Result<List<HolodexFavoriteDto>>> Handle(GetFollowedChannelsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var followed = await db.FollowedChannels
            .Where(c => c.ProfileId == profileId)
            .Select(c => new HolodexFavoriteDto(c.HolodexChannelId, c.Name, c.EnglishName, c.PhotoUrl, true))
            .ToListAsync(cancellationToken);

        return Result<List<HolodexFavoriteDto>>.Success(followed);
    }
}
