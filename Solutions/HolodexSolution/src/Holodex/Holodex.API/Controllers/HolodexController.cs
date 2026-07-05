using Holodex.Application.Features.Holodex.Commands.SaveFollowedChannels;
using Holodex.Application.Features.Holodex.Commands.StoreHolodexCredential;
using Holodex.Application.Features.Holodex.Commands.TriggerHolodexSync;
using Holodex.Application.Features.Holodex.Queries.GetFollowedChannels;
using Holodex.Application.Features.Holodex.Queries.GetHolodexStatus;
using Holodex.Application.Features.Holodex.Queries.SearchHolodexChannels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Holodex.API.Controllers;

[ApiController]
[Route("api/holodex")]
[Authorize]
public class HolodexController(ISender sender) : ControllerBase
{
    [HttpPost("credential")]
    public async Task<IActionResult> StoreCredential([FromBody] StoreHolodexCredentialCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchChannels([FromQuery] string q, CancellationToken ct)
    {
        var result = await sender.Send(new SearchHolodexChannelsQuery(q), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("followed")]
    public async Task<IActionResult> GetFollowedChannels(CancellationToken ct)
    {
        var result = await sender.Send(new GetFollowedChannelsQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost("followed")]
    public async Task<IActionResult> SaveFollowedChannels([FromBody] SaveFollowedChannelsCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Ok(new { count = result.Value }) : BadRequest(result.Errors);
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus(CancellationToken ct)
    {
        var result = await sender.Send(new GetHolodexStatusQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> TriggerSync(CancellationToken ct)
    {
        var result = await sender.Send(new TriggerHolodexSyncCommand(), ct);
        return result.Succeeded ? Accepted() : BadRequest(result.Errors);
    }
}
