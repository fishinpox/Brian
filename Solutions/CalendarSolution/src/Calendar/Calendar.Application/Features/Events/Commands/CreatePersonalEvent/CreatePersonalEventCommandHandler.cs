using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using MediatR;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Events.Commands.CreatePersonalEvent;

public class CreatePersonalEventCommandHandler(
    ICalendarDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<CreatePersonalEventCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePersonalEventCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var personalEvent = PersonalEvent.Create(
            currentUser.ProfileId.Value,
            request.Title,
            request.Description,
            request.Location,
            request.StartAt,
            request.EndAt,
            request.IsAllDay,
            request.RecurrenceRule);

        db.PersonalEvents.Add(personalEvent);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(personalEvent.Id);
    }
}
