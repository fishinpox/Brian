using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Notes.Commands.CreateNote;

public record CreateNoteCommand(string Title, string Content) : IRequest<Result<NoteDto>>;
