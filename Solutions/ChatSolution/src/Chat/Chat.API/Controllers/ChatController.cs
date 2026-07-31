using Chat.Application.Features.Accounts.Commands.EnsureChatAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController(ISender sender) : ControllerBase
{
    [HttpPost("account")]
    public async Task<IActionResult> EnsureAccount(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new EnsureChatAccountCommand(), cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }
}
