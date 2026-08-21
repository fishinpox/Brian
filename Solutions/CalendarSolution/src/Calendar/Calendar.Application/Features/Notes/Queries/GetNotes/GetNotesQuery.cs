using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Notes.Queries.GetNotes;

public record GetNotesQuery : IRequest<Result<List<NoteDto>>>;
