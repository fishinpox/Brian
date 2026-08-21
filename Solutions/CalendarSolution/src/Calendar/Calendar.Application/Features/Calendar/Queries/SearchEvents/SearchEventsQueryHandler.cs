using Calendar.Application.Common;
using Calendar.Application.Common.Interfaces;
using Calendar.Application.Features.Calendar.Queries.GetCalendarView;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Calendar.Queries.SearchEvents;

public class SearchEventsQueryHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SearchEventsQuery, Result<List<PersonalEventDto>>>
{
    public async Task<Result<List<PersonalEventDto>>> Handle(SearchEventsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var matches = await db.PersonalEvents
            .Where(e => e.ProfileId == profileId && e.Title.Contains(request.Keyword))
            .OrderByDescending(e => e.StartAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        var lockLookup = await BulkFolderLockLookup.LoadAsync(db, profileId, cancellationToken);

        var dtos = matches
            .Select(e => new PersonalEventDto(
                e.Id,
                e.Title,
                e.Description,
                e.Location,
                e.StartAt,
                e.EndAt,
                e.IsAllDay,
                e.Status.ToString(),
                e.SubfolderId,
                e.IsVisible,
                lockLookup.IsDraggable(e.SubfolderId),
                e.IsCompleted,
                e.CountdownCategory?.ToString(),
                e.RecurrenceType.ToString(),
                e.RecurrenceEndDate,
                e.AutoDeferEnabled))
            .ToList();

        return Result<List<PersonalEventDto>>.Success(dtos);
    }
}
