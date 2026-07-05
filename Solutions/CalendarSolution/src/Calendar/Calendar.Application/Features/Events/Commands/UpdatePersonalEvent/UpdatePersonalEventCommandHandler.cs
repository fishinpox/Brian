using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Events.Commands.UpdatePersonalEvent;

public class UpdatePersonalEventCommandHandler(
    ICalendarDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdatePersonalEventCommand, Result>
{
    public async Task<Result> Handle(UpdatePersonalEventCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var personalEvent = await db.PersonalEvents
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.PersonalEvent), request.EventId);

        if (personalEvent.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        personalEvent.Update(
            request.Title,
            request.Description,
            request.Location,
            request.StartAt,
            request.EndAt,
            request.IsAllDay,
            request.RecurrenceRule);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
