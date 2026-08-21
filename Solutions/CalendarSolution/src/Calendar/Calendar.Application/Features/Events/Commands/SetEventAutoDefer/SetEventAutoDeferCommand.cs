using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.SetEventAutoDefer;

public record SetEventAutoDeferCommand(Guid EventId, bool AutoDeferEnabled) : IRequest<Result>;
