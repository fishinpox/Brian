using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.UpdatePersonalEvent;

public record UpdatePersonalEventCommand(
    Guid EventId,
    string Title,
    string? Description,
    string? Location,
    DateTimeOffset StartAt,
    DateTimeOffset? EndAt,
    bool IsAllDay,
    string? RecurrenceRule) : IRequest<Result>;
