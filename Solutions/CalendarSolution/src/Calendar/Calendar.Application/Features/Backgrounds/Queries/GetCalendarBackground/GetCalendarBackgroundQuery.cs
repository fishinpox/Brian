using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Backgrounds.Queries.GetCalendarBackground;

public record GetCalendarBackgroundQuery : IRequest<Result<CalendarBackgroundFileDto>>;
