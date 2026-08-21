using Calendar.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.SetEventRecurrence;

public record SetEventRecurrenceCommand(Guid EventId, RecurrenceType RecurrenceType, DateTimeOffset? RecurrenceEndDate) : IRequest<Result>;
