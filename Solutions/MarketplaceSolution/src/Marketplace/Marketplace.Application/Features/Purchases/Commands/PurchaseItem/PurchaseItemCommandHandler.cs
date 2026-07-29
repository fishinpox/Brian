using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events.Marketplace;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Marketplace.Application.Features.Purchases.Commands.PurchaseItem;

public class PurchaseItemCommandHandler(
    IMarketplaceDbContext db,
    ICurrentUserService currentUser,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<PurchaseItemCommand, Result<PurchaseItemResponse>>
{
    public async Task<Result<PurchaseItemResponse>> Handle(PurchaseItemCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var item = await db.WallpaperItems
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && i.IsActive, cancellationToken);
        if (item is null)
            return Result<PurchaseItemResponse>.Failure("Wallpaper not found.");

        // Idempotent: no payment gate means a double-click "Buy" is genuinely reachable.
        var existing = await db.WallpaperOwnerships
            .FirstOrDefaultAsync(o => o.ProfileId == profileId && o.ItemId == request.ItemId, cancellationToken);
        if (existing is not null)
            return Result<PurchaseItemResponse>.Success(
                new PurchaseItemResponse(existing.Id, existing.ItemId, existing.PurchasedAt));

        var ownership = WallpaperOwnership.Purchase(profileId, request.ItemId);
        db.WallpaperOwnerships.Add(ownership);
        await db.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new WallpaperOwnershipGrantedEvent(
            ownership.Id, profileId, request.ItemId, DateTimeOffset.UtcNow), cancellationToken);

        return Result<PurchaseItemResponse>.Success(
            new PurchaseItemResponse(ownership.Id, ownership.ItemId, ownership.PurchasedAt));
    }
}
