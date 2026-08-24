using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class CalendarSettings : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public bool MasterLockEnabled { get; private set; }
    public bool LockFoldersByDefault { get; private set; }
    public bool AutoRecolorByTimeSensitivity { get; private set; }
    public int DueSoonWindowHours { get; private set; } = 24;
    public bool TransparentBackground { get; private set; }

    private CalendarSettings() { }

    public static CalendarSettings CreateDefault(Guid profileId)
    {
        return new CalendarSettings
        {
            ProfileId = profileId,
            MasterLockEnabled = false,
            LockFoldersByDefault = false,
            AutoRecolorByTimeSensitivity = false,
            DueSoonWindowHours = 24,
            TransparentBackground = false
        };
    }

    public void Update(bool masterLockEnabled, bool lockFoldersByDefault, bool autoRecolorByTimeSensitivity, int dueSoonWindowHours, bool transparentBackground)
    {
        MasterLockEnabled = masterLockEnabled;
        LockFoldersByDefault = lockFoldersByDefault;
        AutoRecolorByTimeSensitivity = autoRecolorByTimeSensitivity;
        DueSoonWindowHours = dueSoonWindowHours;
        TransparentBackground = transparentBackground;
    }
}
