using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Folders.Commands.SetFolderVisibility;

public class SetFolderVisibilityCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetFolderVisibilityCommand, Result>
{
    public async Task<Result> Handle(SetFolderVisibilityCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == request.FolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Folder), request.FolderId);

        if (folder.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        folder.SetVisibility(request.IsVisible);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
