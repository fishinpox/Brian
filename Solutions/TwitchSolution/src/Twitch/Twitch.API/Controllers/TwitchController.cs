using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twitch.Application.Features.Twitch.Commands.StoreTwitchCredential;

namespace Twitch.API.Controllers;

[ApiController]
[Route("api/twitch")]
[Authorize]
public class TwitchController(ISender sender) : ControllerBase
{
    [HttpPost("credential")]
    public async Task<IActionResult> StoreCredential([FromBody] StoreTwitchCredentialCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
