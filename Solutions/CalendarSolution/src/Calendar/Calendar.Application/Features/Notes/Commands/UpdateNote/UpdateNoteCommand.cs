using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Notes.Commands.UpdateNote;

public record UpdateNoteCommand(Guid NoteId, string Title, string Content) : IRequest<Result>;
