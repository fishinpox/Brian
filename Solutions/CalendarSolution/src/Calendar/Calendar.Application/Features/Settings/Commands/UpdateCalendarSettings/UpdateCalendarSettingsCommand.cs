using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Settings.Commands.UpdateCalendarSettings;

public record UpdateCalendarSettingsCommand(
    bool MasterLockEnabled,
    bool LockFoldersByDefault,
    bool AutoRecolorByTimeSensitivity,
    int DueSoonWindowHours,
    bool TransparentBackground) : IRequest<Result<CalendarSettingsDto>>;
