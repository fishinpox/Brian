using Holodex.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Holodex.Application.Features.Holodex.Queries.GetHolodexStatus;

public class GetHolodexStatusQueryHandler(
    IHolodexDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetHolodexStatusQuery, Result<HolodexStatusDto>>
{
    public async Task<Result<HolodexStatusDto>> Handle(GetHolodexStatusQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var hasCredential = await db.ExternalCredentials
            .AnyAsync(c => c.ProfileId == profileId, cancellationToken);

        var followedCount = await db.FollowedChannels
            .CountAsync(c => c.ProfileId == profileId, cancellationToken);

        return Result<HolodexStatusDto>.Success(new HolodexStatusDto(hasCredential, followedCount));
    }
}
