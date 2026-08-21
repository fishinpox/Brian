using Calendar.Application.Common;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Folders.Commands.SetFolderLock;

public class SetFolderLockCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetFolderLockCommand, Result>
{
    public async Task<Result> Handle(SetFolderLockCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == request.FolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Folder), request.FolderId);

        if (folder.ProfileId != profileId)
            throw new ForbiddenAccessException();

        var settings = await CalendarSettingsHelper.GetOrCreateAsync(db, profileId, cancellationToken);
        if (settings.MasterLockEnabled)
            return Result.Failure("Master Lock is on - individual folder locks can't be changed while it's enabled.");

        folder.SetLock(request.IsLocked);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
