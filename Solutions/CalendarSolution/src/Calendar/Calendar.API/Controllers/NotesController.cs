using Calendar.Application.Features.Notes.Commands.CreateNote;
using Calendar.Application.Features.Notes.Commands.DeleteNote;
using Calendar.Application.Features.Notes.Commands.UpdateNote;
using Calendar.Application.Features.Notes.Queries.GetNotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Controllers;

public record UpdateNoteRequest(string Title, string Content);

[ApiController]
[Route("api/calendar/notes")]
[Authorize]
public class NotesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotes(CancellationToken ct)
    {
        var result = await sender.Send(new GetNotesQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateNoteCommand(id, request.Title, request.Content), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteNoteCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
