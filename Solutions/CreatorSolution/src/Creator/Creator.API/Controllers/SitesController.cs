using Creator.Application.Features.Sites.Commands.ProvisionSite;
using Creator.Application.Features.Sites.Commands.PublishSite;
using Creator.Application.Features.Sites.Commands.UnpublishSite;
using Creator.Application.Features.Sites.Queries.GetMySite;
using Creator.Application.Features.Sites.Queries.GetSiteBySlug;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Creator.API.Controllers;

[ApiController]
[Route("api/creators/sites")]
[Authorize]
public class SitesController(ISender sender) : ControllerBase
{
    [HttpPost("provision")]
    [Authorize(Roles = "Creator")]
    public async Task<IActionResult> Provision([FromBody] ProvisionSiteCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMySiteQuery(), cancellationToken);
        return result.Failed ? NotFound(result.Errors) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new PublishSiteCommand(id), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UnpublishSiteCommand(id), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSiteBySlugQuery(slug), cancellationToken);
        return result.Failed ? NotFound(result.Errors) : Ok(result.Value);
    }
}
