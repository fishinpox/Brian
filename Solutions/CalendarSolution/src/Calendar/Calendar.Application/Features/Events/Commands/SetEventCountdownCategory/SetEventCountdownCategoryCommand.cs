using Calendar.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.SetEventCountdownCategory;

public record SetEventCountdownCategoryCommand(Guid EventId, CountdownCategory? Category) : IRequest<Result>;
