using Calendar.Application.Common;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Subfolders.Commands.SetSubfolderLock;

public class SetSubfolderLockCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetSubfolderLockCommand, Result>
{
    public async Task<Result> Handle(SetSubfolderLockCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var subfolder = await db.Subfolders.FirstOrDefaultAsync(s => s.Id == request.SubfolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Subfolder), request.SubfolderId);

        if (subfolder.ProfileId != profileId)
            throw new ForbiddenAccessException();

        var settings = await CalendarSettingsHelper.GetOrCreateAsync(db, profileId, cancellationToken);
        if (settings.MasterLockEnabled)
            return Result.Failure("Master Lock is on - subfolder locks can't be changed while it's enabled.");

        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == subfolder.FolderId, cancellationToken);
        if (folder?.IsLocked == true)
            return Result.Failure("This subfolder's master folder is locked - its own lock can't be changed until the master folder is unlocked.");

        subfolder.SetLock(request.IsLocked);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
