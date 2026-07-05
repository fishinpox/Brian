using Calendar.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Reminders.Commands.SetReminder;

public record SetReminderCommand(
    Guid PersonalEventId,
    DateTimeOffset TriggerAt,
    ReminderMethod Method) : IRequest<Result<Guid>>;
