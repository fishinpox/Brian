using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Folders.Commands.DeleteFolder;

public class DeleteFolderCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteFolderCommand, Result>
{
    public async Task<Result> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == request.FolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Folder), request.FolderId);

        if (folder.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        var subfolders = await db.Subfolders.Where(s => s.FolderId == folder.Id).ToListAsync(cancellationToken);
        var subfolderIds = subfolders.Select(s => s.Id).ToList();

        // Deleting a folder never destroys events - it only un-assigns them.
        var affectedEvents = await db.PersonalEvents
            .Where(e => e.SubfolderId != null && subfolderIds.Contains(e.SubfolderId!.Value))
            .ToListAsync(cancellationToken);
        foreach (var personalEvent in affectedEvents)
            personalEvent.AssignToSubfolder(null);

        db.Subfolders.RemoveRange(subfolders);
        db.Folders.Remove(folder);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
