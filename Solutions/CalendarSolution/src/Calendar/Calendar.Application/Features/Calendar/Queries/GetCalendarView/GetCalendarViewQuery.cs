using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Calendar.Queries.GetCalendarView;

public record GetCalendarViewQuery(DateTimeOffset From, DateTimeOffset To) : IRequest<Result<CalendarViewDto>>;
