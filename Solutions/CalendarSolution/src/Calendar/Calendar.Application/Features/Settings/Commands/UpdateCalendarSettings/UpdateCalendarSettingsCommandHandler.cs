using Calendar.Application.Common;
using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Settings.Commands.UpdateCalendarSettings;

public class UpdateCalendarSettingsCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateCalendarSettingsCommand, Result<CalendarSettingsDto>>
{
    public async Task<Result<CalendarSettingsDto>> Handle(UpdateCalendarSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var settings = await CalendarSettingsHelper.GetOrCreateAsync(db, currentUser.ProfileId.Value, cancellationToken);
        settings.Update(request.MasterLockEnabled, request.LockFoldersByDefault, request.AutoRecolorByTimeSensitivity, request.DueSoonWindowHours);
        await db.SaveChangesAsync(cancellationToken);

        return Result<CalendarSettingsDto>.Success(new CalendarSettingsDto(
            settings.MasterLockEnabled, settings.LockFoldersByDefault, settings.AutoRecolorByTimeSensitivity, settings.DueSoonWindowHours));
    }
}
