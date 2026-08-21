using Calendar.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Application.Common;

/// <summary>
/// Single-event lock check for command handlers (move/reassign). Query handlers that return many
/// events (GetFolderTree, GetCalendarView) bulk-load folders/subfolders/settings themselves and
/// compute this in-memory instead, to avoid N+1 queries.
/// </summary>
public static class FolderLockEvaluator
{
    public static async Task<bool> IsEventDraggableAsync(ICalendarDbContext db, Guid profileId, Guid? subfolderId, CancellationToken cancellationToken)
    {
        var settings = await db.CalendarSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId, cancellationToken);
        if (settings?.MasterLockEnabled == true)
            return false;

        if (subfolderId is null)
            return true;

        var subfolder = await db.Subfolders.FirstOrDefaultAsync(s => s.Id == subfolderId, cancellationToken);
        if (subfolder is null)
            return true;
        if (subfolder.IsLocked)
            return false;

        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == subfolder.FolderId, cancellationToken);
        return folder is null || !folder.IsLocked;
    }
}
