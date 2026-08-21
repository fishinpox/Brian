using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Notes.Commands.UpdateNote;

public class UpdateNoteCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateNoteCommand, Result>
{
    public async Task<Result> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == request.NoteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Note), request.NoteId);

        if (note.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        note.Update(request.Title, request.Content);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
