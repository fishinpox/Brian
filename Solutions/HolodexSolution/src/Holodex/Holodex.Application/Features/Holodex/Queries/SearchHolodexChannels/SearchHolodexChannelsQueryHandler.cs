using Holodex.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Holodex.Application.Features.Holodex.Queries.SearchHolodexChannels;

public class SearchHolodexChannelsQueryHandler(
    IHolodexDbContext db,
    ICurrentUserService currentUser,
    IHolodexApiClient holodexClient)
    : IRequestHandler<SearchHolodexChannelsQuery, Result<List<HolodexFavoriteDto>>>
{
    public async Task<Result<List<HolodexFavoriteDto>>> Handle(SearchHolodexChannelsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var credential = await db.ExternalCredentials
            .FirstOrDefaultAsync(c => c.ProfileId == profileId, cancellationToken);

        if (credential is null)
            return Result<List<HolodexFavoriteDto>>.Failure("No Holodex account linked");

        var apiKey = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(credential.EncryptedValue));

        var channels = await holodexClient.SearchChannelsAsync(apiKey, request.SearchTerm);

        var followedIds = await db.FollowedChannels
            .Where(c => c.ProfileId == profileId)
            .Select(c => c.HolodexChannelId)
            .ToListAsync(cancellationToken);
        var followedSet = followedIds.ToHashSet();

        var results = channels
            .Select(c => new HolodexFavoriteDto(c.Id, c.Name, c.English_name, c.Photo, followedSet.Contains(c.Id)))
            .ToList();

        return Result<List<HolodexFavoriteDto>>.Success(results);
    }
}
