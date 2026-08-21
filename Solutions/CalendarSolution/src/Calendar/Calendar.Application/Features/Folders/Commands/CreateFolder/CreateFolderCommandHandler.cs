using Calendar.Application.Common;
using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Constants;
using Calendar.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Folders.Commands.CreateFolder;

public class CreateFolderCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateFolderCommand, Result<FolderDto>>
{
    public async Task<Result<FolderDto>> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;
        var settings = await CalendarSettingsHelper.GetOrCreateAsync(db, profileId, cancellationToken);

        var existingCount = await db.Folders.CountAsync(f => f.ProfileId == profileId, cancellationToken);
        var color = FolderPalette.NextColor(existingCount);

        var folder = Folder.Create(profileId, request.Name, color.Background, color.Text, color.Border, settings.LockFoldersByDefault);
        db.Folders.Add(folder);
        await db.SaveChangesAsync(cancellationToken);

        return Result<FolderDto>.Success(new FolderDto(
            folder.Id, folder.Name, folder.ColorBackground, folder.ColorText, folder.ColorBorder,
            folder.IsVisible, folder.IsLocked, []));
    }
}
