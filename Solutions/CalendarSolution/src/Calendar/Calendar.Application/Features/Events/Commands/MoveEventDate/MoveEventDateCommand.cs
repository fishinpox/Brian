using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.MoveEventDate;

public record MoveEventDateCommand(Guid EventId, DateTimeOffset NewStartAt) : IRequest<Result>;
