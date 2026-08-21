using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.SetEventVisibility;

public record SetEventVisibilityCommand(Guid EventId, bool IsVisible) : IRequest<Result>;
