using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Subfolders.Commands.DeleteSubfolder;

public class DeleteSubfolderCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteSubfolderCommand, Result>
{
    public async Task<Result> Handle(DeleteSubfolderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var subfolder = await db.Subfolders.FirstOrDefaultAsync(s => s.Id == request.SubfolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Subfolder), request.SubfolderId);

        if (subfolder.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        var affectedEvents = await db.PersonalEvents
            .Where(e => e.SubfolderId == subfolder.Id)
            .ToListAsync(cancellationToken);
        foreach (var personalEvent in affectedEvents)
            personalEvent.AssignToSubfolder(null);

        db.Subfolders.Remove(subfolder);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
