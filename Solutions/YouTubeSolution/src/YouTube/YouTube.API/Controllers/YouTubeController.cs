using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouTube.Application.Features.YouTube.Commands.StoreYouTubeCredential;

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
}
