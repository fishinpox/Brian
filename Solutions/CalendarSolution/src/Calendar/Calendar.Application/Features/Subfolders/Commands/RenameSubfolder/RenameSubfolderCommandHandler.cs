using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Subfolders.Commands.RenameSubfolder;

public class RenameSubfolderCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<RenameSubfolderCommand, Result>
{
    public async Task<Result> Handle(RenameSubfolderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var subfolder = await db.Subfolders.FirstOrDefaultAsync(s => s.Id == request.SubfolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Subfolder), request.SubfolderId);

        if (subfolder.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        subfolder.Rename(request.Name);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
