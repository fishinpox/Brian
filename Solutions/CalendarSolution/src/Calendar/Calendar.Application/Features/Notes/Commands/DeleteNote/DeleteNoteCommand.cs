using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Notes.Commands.DeleteNote;

public record DeleteNoteCommand(Guid NoteId) : IRequest<Result>;
