using Calendar.Application.Common;
using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Folders.Queries.GetFolderTree;

public class GetFolderTreeQueryHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetFolderTreeQuery, Result<FolderTreeDto>>
{
    public async Task<Result<FolderTreeDto>> Handle(GetFolderTreeQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        // Bulk-load everything for this profile up front and compute lock state in-memory,
        // rather than N+1 querying per event - a profile's folder tree is small.
        var settings = await CalendarSettingsHelper.GetOrCreateAsync(db, profileId, cancellationToken);
        var folders = await db.Folders.Where(f => f.ProfileId == profileId).OrderBy(f => f.CreatedAt).ToListAsync(cancellationToken);
        var subfolders = await db.Subfolders.Where(s => s.ProfileId == profileId).OrderBy(s => s.CreatedAt).ToListAsync(cancellationToken);
        var subfolderIds = subfolders.Select(s => s.Id).ToList();
        var events = await db.PersonalEvents
            .Where(e => e.SubfolderId != null && subfolderIds.Contains(e.SubfolderId!.Value))
            .OrderBy(e => e.StartAt)
            .ToListAsync(cancellationToken);

        var folderDtos = folders.Select(folder =>
        {
            var subfolderDtos = subfolders
                .Where(s => s.FolderId == folder.Id)
                .Select(subfolder =>
                {
                    var isDraggable = !settings.MasterLockEnabled && !folder.IsLocked && !subfolder.IsLocked;
                    var eventDtos = events
                        .Where(e => e.SubfolderId == subfolder.Id)
                        .Select(e => new FolderedEventDto(e.Id, e.Title, e.StartAt, e.IsVisible, isDraggable, e.IsCompleted))
                        .ToList();

                    return new SubfolderDto(subfolder.Id, subfolder.FolderId, subfolder.Name, subfolder.IsVisible, subfolder.IsLocked, eventDtos);
                })
                .ToList();

            return new FolderDto(
                folder.Id, folder.Name, folder.ColorBackground, folder.ColorText, folder.ColorBorder,
                folder.IsVisible, folder.IsLocked, subfolderDtos);
        }).ToList();

        return Result<FolderTreeDto>.Success(new FolderTreeDto(settings.MasterLockEnabled, folderDtos));
    }
}
