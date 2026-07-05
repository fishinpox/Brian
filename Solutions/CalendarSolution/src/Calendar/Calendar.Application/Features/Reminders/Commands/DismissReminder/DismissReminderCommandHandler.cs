using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Reminders.Commands.DismissReminder;

public class DismissReminderCommandHandler(
    ICalendarDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<DismissReminderCommand, Result>
{
    public async Task<Result> Handle(DismissReminderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.Id == request.ReminderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Reminder), request.ReminderId);

        if (reminder.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        reminder.IsSent = true;
        reminder.SentAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
