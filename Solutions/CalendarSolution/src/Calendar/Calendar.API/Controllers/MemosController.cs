using Calendar.Application.Features.Memos.Commands.CreateMemo;
using Calendar.Application.Features.Memos.Commands.DeleteMemo;
using Calendar.Application.Features.Memos.Queries.GetMemos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Controllers;

[ApiController]
[Route("api/calendar/memos")]
[Authorize]
public class MemosController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMemos([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken ct)
    {
        var result = await sender.Send(new GetMemosQuery(from, to), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMemoCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteMemoCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
