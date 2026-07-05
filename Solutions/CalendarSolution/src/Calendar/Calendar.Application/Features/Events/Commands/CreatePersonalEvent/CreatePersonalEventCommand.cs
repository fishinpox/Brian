using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.CreatePersonalEvent;

public record CreatePersonalEventCommand(
    string Title,
    string? Description,
    string? Location,
    DateTimeOffset StartAt,
    DateTimeOffset? EndAt,
    bool IsAllDay,
    string? RecurrenceRule) : IRequest<Result<Guid>>;
