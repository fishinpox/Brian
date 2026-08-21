using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class CalendarSettings : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public bool MasterLockEnabled { get; private set; }
    public bool LockFoldersByDefault { get; private set; }
    public bool AutoRecolorByTimeSensitivity { get; private set; }
    public int DueSoonWindowHours { get; private set; } = 24;

    private CalendarSettings() { }

    public static CalendarSettings CreateDefault(Guid profileId)
    {
        return new CalendarSettings
        {
            ProfileId = profileId,
            MasterLockEnabled = false,
            LockFoldersByDefault = false,
            AutoRecolorByTimeSensitivity = false,
            DueSoonWindowHours = 24
        };
    }

    public void Update(bool masterLockEnabled, bool lockFoldersByDefault, bool autoRecolorByTimeSensitivity, int dueSoonWindowHours)
    {
        MasterLockEnabled = masterLockEnabled;
        LockFoldersByDefault = lockFoldersByDefault;
        AutoRecolorByTimeSensitivity = autoRecolorByTimeSensitivity;
        DueSoonWindowHours = dueSoonWindowHours;
    }
}
