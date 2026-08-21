using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Events.Commands.SetEventRecurrence;

public class SetEventRecurrenceCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetEventRecurrenceCommand, Result>
{
    public async Task<Result> Handle(SetEventRecurrenceCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var personalEvent = await db.PersonalEvents
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.PersonalEvent), request.EventId);

        if (personalEvent.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        personalEvent.SetRecurrence(request.RecurrenceType, request.RecurrenceEndDate);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
