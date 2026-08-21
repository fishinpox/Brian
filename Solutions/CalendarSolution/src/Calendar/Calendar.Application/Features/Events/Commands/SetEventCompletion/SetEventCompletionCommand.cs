using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.SetEventCompletion;

public record SetEventCompletionCommand(Guid EventId, bool IsCompleted) : IRequest<Result>;
