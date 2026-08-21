using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Folders.Commands.SetFolderColor;

public class SetFolderColorCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetFolderColorCommand, Result>
{
    public async Task<Result> Handle(SetFolderColorCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == request.FolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Folder), request.FolderId);

        if (folder.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        folder.SetColor(request.Background, request.Text, request.Border);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
