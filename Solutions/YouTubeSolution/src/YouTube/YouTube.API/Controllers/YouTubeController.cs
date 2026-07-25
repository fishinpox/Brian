using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouTube.Application.Features.YouTube.Commands.SaveFollowedChannels;
using YouTube.Application.Features.YouTube.Commands.StoreYouTubeCredential;
using YouTube.Application.Features.YouTube.Queries.GetFollowedChannels;
using YouTube.Application.Features.YouTube.Queries.GetYouTubeStatus;

namespace YouTube.API.Controllers;

[ApiController]
[Route("api/youtube")]
[Authorize]
public class YouTubeController(ISender sender) : ControllerBase
{
    [HttpPost("credential")]
    public async Task<IActionResult> StoreCredential([FromBody] StoreYouTubeCredentialCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
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
        var result = await sender.Send(new GetYouTubeStatusQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
