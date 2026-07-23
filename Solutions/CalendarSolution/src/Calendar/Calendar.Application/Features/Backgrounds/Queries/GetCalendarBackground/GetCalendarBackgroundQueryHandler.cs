using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Backgrounds.Queries.GetCalendarBackground;

public class GetCalendarBackgroundQueryHandler(
    ICalendarDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetCalendarBackgroundQuery, Result<CalendarBackgroundFileDto>>
{
    public async Task<Result<CalendarBackgroundFileDto>> Handle(GetCalendarBackgroundQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var background = await db.CalendarBackgrounds
            .FirstOrDefaultAsync(b => b.ProfileId == currentUser.ProfileId.Value
                && b.Status == CalendarBackgroundStatus.Approved, cancellationToken);

        return background is null
            ? Result<CalendarBackgroundFileDto>.Failure("No approved background is set.")
            : Result<CalendarBackgroundFileDto>.Success(
                new CalendarBackgroundFileDto(background.ImageData, background.ContentType, background.FileName));
    }
}
