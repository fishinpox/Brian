using Calendar.Application.Features.Calendar.Queries.GetCalendarView;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Calendar.Queries.SearchEvents;

public record SearchEventsQuery(string Keyword) : IRequest<Result<List<PersonalEventDto>>>;
