using Marketplace.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Marketplace.Application.Features.Ownership.Queries.GetPendingOwnerships;

public class GetPendingOwnershipsQueryHandler(IMarketplaceDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetPendingOwnershipsQuery, Result<List<PendingOwnershipDto>>>
{
    public async Task<Result<List<PendingOwnershipDto>>> Handle(GetPendingOwnershipsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var pending = await (
            from ownership in db.WallpaperOwnerships
            join item in db.WallpaperItems on ownership.ItemId equals item.Id
            where ownership.ProfileId == currentUser.ProfileId.Value && ownership.AppliedAt == null
            select new PendingOwnershipDto(ownership.Id, item.Id, item.Name, ownership.PurchasedAt)
        ).ToListAsync(cancellationToken);

        return Result<List<PendingOwnershipDto>>.Success(pending);
    }
}
