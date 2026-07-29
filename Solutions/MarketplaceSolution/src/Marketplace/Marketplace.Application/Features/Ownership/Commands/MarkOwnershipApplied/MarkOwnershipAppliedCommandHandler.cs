using Marketplace.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Marketplace.Application.Features.Ownership.Commands.MarkOwnershipApplied;

public class MarkOwnershipAppliedCommandHandler(IMarketplaceDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<MarkOwnershipAppliedCommand, Result>
{
    public async Task<Result> Handle(MarkOwnershipAppliedCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var ownership = await db.WallpaperOwnerships
            .FirstOrDefaultAsync(o => o.Id == request.OwnershipId && o.ProfileId == currentUser.ProfileId.Value, cancellationToken);

        if (ownership is null)
            return Result.Failure("Ownership not found.");

        if (ownership.AppliedAt is null)
        {
            ownership.MarkApplied();
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
