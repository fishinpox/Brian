using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using MediatR;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Notes.Commands.CreateNote;

public class CreateNoteCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateNoteCommand, Result<NoteDto>>
{
    public async Task<Result<NoteDto>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var note = Note.Create(currentUser.ProfileId.Value, request.Title, request.Content);
        db.Notes.Add(note);
        await db.SaveChangesAsync(cancellationToken);

        return Result<NoteDto>.Success(new NoteDto(note.Id, note.Title, note.Content, note.CreatedAt, note.UpdatedAt));
    }
}
