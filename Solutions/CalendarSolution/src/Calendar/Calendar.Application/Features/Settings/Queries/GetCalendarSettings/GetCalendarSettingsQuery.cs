using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Settings.Queries.GetCalendarSettings;

public record GetCalendarSettingsQuery : IRequest<Result<CalendarSettingsDto>>;
