using Identity.Application.Features.Profiles.Commands.CreateProfile;
using Identity.Application.Features.Profiles.Commands.SwitchActiveProfile;
using Identity.Application.Features.Profiles.Queries.GetAccountProfiles;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/profiles")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfilesController(ISender sender) : ControllerBase
{
    private string? ClientIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();
    private string? ClientUserAgent => Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : null;

    [HttpGet]
    public async Task<IActionResult> GetProfiles(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAccountProfilesQuery(), cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileCommand command, CancellationToken cancellationToken)
    {
        var enriched = command with { IpAddress = ClientIpAddress, UserAgent = ClientUserAgent };
        var result = await sender.Send(enriched, cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateProfile(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SwitchActiveProfileCommand(id), cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }
}
