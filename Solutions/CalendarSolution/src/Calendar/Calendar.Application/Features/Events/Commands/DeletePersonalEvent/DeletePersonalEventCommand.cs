using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.DeletePersonalEvent;

public record DeletePersonalEventCommand(Guid EventId) : IRequest<Result>;
