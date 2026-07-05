using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Reminders.Commands.SetReminder;

public class SetReminderCommandHandler(
    ICalendarDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<SetReminderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SetReminderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var personalEvent = await db.PersonalEvents
            .FirstOrDefaultAsync(e => e.Id == request.PersonalEventId, cancellationToken)
            ?? throw new NotFoundException(nameof(PersonalEvent), request.PersonalEventId);

        if (personalEvent.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        var reminder = Reminder.Create(
            request.PersonalEventId,
            currentUser.ProfileId.Value,
            request.TriggerAt,
            request.Method);

        db.Reminders.Add(reminder);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(reminder.Id);
    }
}
