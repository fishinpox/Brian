using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Subfolders.Commands.SetSubfolderVisibility;

public class SetSubfolderVisibilityCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetSubfolderVisibilityCommand, Result>
{
    public async Task<Result> Handle(SetSubfolderVisibilityCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var subfolder = await db.Subfolders.FirstOrDefaultAsync(s => s.Id == request.SubfolderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Subfolder), request.SubfolderId);

        if (subfolder.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        subfolder.SetVisibility(request.IsVisible);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
