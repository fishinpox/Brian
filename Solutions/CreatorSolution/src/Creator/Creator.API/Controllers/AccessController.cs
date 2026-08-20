using Creator.Application.Features.Access.Commands.GrantAccess;
using Creator.Application.Features.Access.Commands.RevokeAccess;
using Creator.Application.Features.Access.Queries.GetSiteAccessGrants;
using Creator.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Creator.API.Controllers;

public record GrantAccessRequest(Guid GranteeProfileId, SiteAccessLevel AccessLevel);

[ApiController]
[Route("api/creators/sites/{siteId:guid}/access")]
[Authorize]
public class AccessController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGrants(Guid siteId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSiteAccessGrantsQuery(siteId), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Grant(Guid siteId, [FromBody] GrantAccessRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GrantAccessCommand(siteId, request.GranteeProfileId, request.AccessLevel), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpDelete("{profileId:guid}")]
    public async Task<IActionResult> Revoke(Guid siteId, Guid profileId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RevokeAccessCommand(siteId, profileId), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : NoContent();
    }
}
