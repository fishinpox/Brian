using Calendar.Application.Features.Folders.Commands.CreateFolder;
using Calendar.Application.Features.Folders.Commands.DeleteFolder;
using Calendar.Application.Features.Folders.Commands.RenameFolder;
using Calendar.Application.Features.Folders.Commands.SetFolderColor;
using Calendar.Application.Features.Folders.Commands.SetFolderLock;
using Calendar.Application.Features.Folders.Commands.SetFolderVisibility;
using Calendar.Application.Features.Folders.Queries.GetFolderTree;
using Calendar.Application.Features.Subfolders.Commands.CreateSubfolder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Controllers;

public record RenameFolderRequest(string Name);
public record SetFolderColorRequest(string Background, string Text, string Border);
public record SetVisibilityRequest(bool IsVisible);
public record SetLockRequest(bool IsLocked);
public record CreateSubfolderRequest(string Name);

[ApiController]
[Route("api/calendar/folders")]
[Authorize]
public class FoldersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTree(CancellationToken ct)
    {
        var result = await sender.Send(new GetFolderTreeQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFolderCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, [FromBody] RenameFolderRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new RenameFolderCommand(id, request.Name), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/color")]
    public async Task<IActionResult> SetColor(Guid id, [FromBody] SetFolderColorRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SetFolderColorCommand(id, request.Background, request.Text, request.Border), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/visibility")]
    public async Task<IActionResult> SetVisibility(Guid id, [FromBody] SetVisibilityRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SetFolderVisibilityCommand(id, request.IsVisible), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/lock")]
    public async Task<IActionResult> SetLock(Guid id, [FromBody] SetLockRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SetFolderLockCommand(id, request.IsLocked), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteFolderCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("{id:guid}/subfolders")]
    public async Task<IActionResult> CreateSubfolder(Guid id, [FromBody] CreateSubfolderRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateSubfolderCommand(id, request.Name), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
