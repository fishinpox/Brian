using Calendar.Application.Common;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Events.Commands.MoveEventToFolder;

public class MoveEventToFolderCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<MoveEventToFolderCommand, Result>
{
    public async Task<Result> Handle(MoveEventToFolderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var personalEvent = await db.PersonalEvents
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.PersonalEvent), request.EventId);

        if (personalEvent.ProfileId != profileId)
            throw new ForbiddenAccessException();

        if (request.SubfolderId is not null)
        {
            var subfolder = await db.Subfolders.FirstOrDefaultAsync(s => s.Id == request.SubfolderId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Subfolder), request.SubfolderId.Value);

            if (subfolder.ProfileId != profileId)
                throw new ForbiddenAccessException();
        }

        var isDraggable = await FolderLockEvaluator.IsEventDraggableAsync(db, profileId, personalEvent.SubfolderId, cancellationToken);
        if (!isDraggable)
            return Result.Failure("This event is locked and can't be moved to a different folder.");

        personalEvent.AssignToSubfolder(request.SubfolderId);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
