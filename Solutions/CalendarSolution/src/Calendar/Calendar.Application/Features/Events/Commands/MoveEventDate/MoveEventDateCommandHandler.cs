using Calendar.Application.Common;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Events.Commands.MoveEventDate;

public class MoveEventDateCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<MoveEventDateCommand, Result>
{
    public async Task<Result> Handle(MoveEventDateCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var personalEvent = await db.PersonalEvents
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.PersonalEvent), request.EventId);

        if (personalEvent.ProfileId != profileId)
            throw new ForbiddenAccessException();

        var isDraggable = await FolderLockEvaluator.IsEventDraggableAsync(db, profileId, personalEvent.SubfolderId, cancellationToken);
        if (!isDraggable)
            return Result.Failure("This event is locked and can't be moved.");

        personalEvent.Reschedule(request.NewStartAt);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
