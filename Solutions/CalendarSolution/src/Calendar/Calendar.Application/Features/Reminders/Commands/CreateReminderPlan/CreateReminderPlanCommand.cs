using Calendar.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Reminders.Commands.CreateReminderPlan;

public record CreateReminderPlanCommand(
    Guid PersonalEventId,
    DateTimeOffset FirstTriggerAt,
    RecurrenceType RecurrenceType,
    DateTimeOffset? EndDate,
    int MaxOccurrences,
    List<TimeSpan> TimesOfDay,
    ReminderMethod Method) : IRequest<Result<List<Guid>>>;
