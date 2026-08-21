using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Notes.Queries.GetNotes;

public class GetNotesQueryHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetNotesQuery, Result<List<NoteDto>>>
{
    public async Task<Result<List<NoteDto>>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var notes = await db.Notes
            .Where(n => n.ProfileId == profileId)
            .OrderByDescending(n => n.UpdatedAt)
            .Select(n => new NoteDto(n.Id, n.Title, n.Content, n.CreatedAt, n.UpdatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<NoteDto>>.Success(notes);
    }
}
