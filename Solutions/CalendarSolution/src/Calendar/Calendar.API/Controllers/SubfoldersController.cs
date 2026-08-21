using Calendar.Application.Features.Subfolders.Commands.DeleteSubfolder;
using Calendar.Application.Features.Subfolders.Commands.RenameSubfolder;
using Calendar.Application.Features.Subfolders.Commands.SetSubfolderLock;
using Calendar.Application.Features.Subfolders.Commands.SetSubfolderVisibility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Controllers;

[ApiController]
[Route("api/calendar/subfolders")]
[Authorize]
public class SubfoldersController(ISender sender) : ControllerBase
{
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, [FromBody] RenameFolderRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new RenameSubfolderCommand(id, request.Name), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/visibility")]
    public async Task<IActionResult> SetVisibility(Guid id, [FromBody] SetVisibilityRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SetSubfolderVisibilityCommand(id, request.IsVisible), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/lock")]
    public async Task<IActionResult> SetLock(Guid id, [FromBody] SetLockRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SetSubfolderLockCommand(id, request.IsLocked), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteSubfolderCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
