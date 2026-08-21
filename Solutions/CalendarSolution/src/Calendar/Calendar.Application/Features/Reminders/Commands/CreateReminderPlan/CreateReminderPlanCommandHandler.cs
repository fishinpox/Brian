using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using Calendar.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Reminders.Commands.CreateReminderPlan;

public class CreateReminderPlanCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateReminderPlanCommand, Result<List<Guid>>>
{
    public async Task<Result<List<Guid>>> Handle(CreateReminderPlanCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var personalEvent = await db.PersonalEvents
            .FirstOrDefaultAsync(e => e.Id == request.PersonalEventId, cancellationToken)
            ?? throw new NotFoundException(nameof(PersonalEvent), request.PersonalEventId);

        if (personalEvent.ProfileId != profileId)
            throw new ForbiddenAccessException();

        var occurrenceDates = new List<DateOnly>();
        var cursor = DateOnly.FromDateTime(request.FirstTriggerAt.UtcDateTime);
        var endDate = request.EndDate.HasValue ? DateOnly.FromDateTime(request.EndDate.Value.UtcDateTime) : (DateOnly?)null;

        for (var i = 0; i < request.MaxOccurrences; i++)
        {
            if (endDate.HasValue && cursor > endDate.Value)
                break;

            occurrenceDates.Add(cursor);

            cursor = request.RecurrenceType switch
            {
                RecurrenceType.Daily => cursor.AddDays(1),
                RecurrenceType.Weekly => cursor.AddDays(7),
                RecurrenceType.Monthly => cursor.AddMonths(1),
                _ => cursor
            };
        }

        var createdIds = new List<Guid>();
        foreach (var date in occurrenceDates)
        {
            foreach (var timeOfDay in request.TimesOfDay)
            {
                var triggerAt = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue).Add(timeOfDay), TimeSpan.Zero);
                var reminder = Reminder.Create(personalEvent.Id, profileId, triggerAt, request.Method);
                db.Reminders.Add(reminder);
                createdIds.Add(reminder.Id);
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result<List<Guid>>.Success(createdIds);
    }
}
