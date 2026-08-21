using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Application.Common;

/// <summary>
/// Bulk-loads a profile's folders/subfolders/settings once and computes per-event lock state
/// in-memory - the shared version of the per-event check in FolderLockEvaluator, for handlers
/// that return many events at once (GetCalendarView, SearchEvents) rather than one.
/// </summary>
public class BulkFolderLockLookup
{
    private readonly bool _masterLockEnabled;
    private readonly Dictionary<Guid, Folder> _folders;
    private readonly Dictionary<Guid, Subfolder> _subfolders;

    private BulkFolderLockLookup(bool masterLockEnabled, Dictionary<Guid, Folder> folders, Dictionary<Guid, Subfolder> subfolders)
    {
        _masterLockEnabled = masterLockEnabled;
        _folders = folders;
        _subfolders = subfolders;
    }

    public static async Task<BulkFolderLockLookup> LoadAsync(ICalendarDbContext db, Guid profileId, CancellationToken cancellationToken)
    {
        var settings = await CalendarSettingsHelper.GetOrCreateAsync(db, profileId, cancellationToken);
        var folders = await db.Folders.Where(f => f.ProfileId == profileId).ToDictionaryAsync(f => f.Id, cancellationToken);
        var subfolders = await db.Subfolders.Where(s => s.ProfileId == profileId).ToDictionaryAsync(s => s.Id, cancellationToken);
        return new BulkFolderLockLookup(settings.MasterLockEnabled, folders, subfolders);
    }

    public bool IsDraggable(Guid? subfolderId)
    {
        if (_masterLockEnabled)
            return false;
        if (subfolderId is null || !_subfolders.TryGetValue(subfolderId.Value, out var subfolder))
            return true;
        if (subfolder.IsLocked)
            return false;

        return !_folders.TryGetValue(subfolder.FolderId, out var folder) || !folder.IsLocked;
    }
}
