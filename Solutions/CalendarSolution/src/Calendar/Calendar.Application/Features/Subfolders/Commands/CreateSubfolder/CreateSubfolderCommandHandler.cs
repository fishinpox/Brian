using Calendar.Application.Common;
using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Subfolders.Commands.CreateSubfolder;

public class CreateSubfolderCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateSubfolderCommand, Result<SubfolderDto>>
{
    public async Task<Result<SubfolderDto>> Handle(CreateSubfolderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == request.FolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Folder), request.FolderId);

        if (folder.ProfileId != profileId)
            throw new ForbiddenAccessException();

        var settings = await CalendarSettingsHelper.GetOrCreateAsync(db, profileId, cancellationToken);

        var subfolder = Subfolder.Create(folder.Id, profileId, request.Name, settings.LockFoldersByDefault);
        db.Subfolders.Add(subfolder);
        await db.SaveChangesAsync(cancellationToken);

        return Result<SubfolderDto>.Success(new SubfolderDto(
            subfolder.Id, subfolder.FolderId, subfolder.Name, subfolder.IsVisible, subfolder.IsLocked, []));
    }
}
