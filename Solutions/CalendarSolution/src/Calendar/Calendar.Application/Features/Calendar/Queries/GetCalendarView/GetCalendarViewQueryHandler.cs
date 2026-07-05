using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Calendar.Queries.GetCalendarView;

public class GetCalendarViewQueryHandler(
    ICalendarDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetCalendarViewQuery, Result<CalendarViewDto>>
{
    public async Task<Result<CalendarViewDto>> Handle(GetCalendarViewQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var personalEvents = await db.PersonalEvents
            .Where(e => e.ProfileId == profileId
                && e.StartAt >= request.From
                && e.StartAt <= request.To)
            .ToListAsync(cancellationToken);

        var streamEvents = await db.StreamEvents
            .Where(e => e.ScheduledStart >= request.From && e.ScheduledStart <= request.To)
            .ToListAsync(cancellationToken);

        var personalEventDtos = personalEvents
            .Select(e => new PersonalEventDto(
                e.Id,
                e.Title,
                e.Description,
                e.Location,
                e.StartAt,
                e.EndAt,
                e.IsAllDay,
                e.Status.ToString()))
            .ToList();

        var streamEventDtos = streamEvents
            .Select(e => new StreamEventDto(
                e.Id,
                e.Title,
                e.ThumbnailUrl,
                e.Platform.ToString(),
                e.ScheduledStart,
                e.Status.ToString(),
                e.Status == EventStatus.Live))
            .ToList();

        return Result<CalendarViewDto>.Success(new CalendarViewDto(personalEventDtos, streamEventDtos));
    }
}
