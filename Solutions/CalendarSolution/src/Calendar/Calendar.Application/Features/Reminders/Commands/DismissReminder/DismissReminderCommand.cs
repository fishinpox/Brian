using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Reminders.Commands.DismissReminder;

public record DismissReminderCommand(Guid ReminderId) : IRequest<Result>;
