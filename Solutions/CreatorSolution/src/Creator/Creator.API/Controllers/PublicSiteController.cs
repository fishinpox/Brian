using Creator.Application.Features.Sites.Queries.GetSiteBySlug;
using Creator.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Creator.API.Controllers;

/// <summary>
/// Backs the Gateway's public vanity-path catch-all route. Placeholder only - real page
/// rendering is a later phase (Part 2 groups B/C); this just proves the slug resolves.
/// </summary>
[ApiController]
[AllowAnonymous]
public class PublicSiteController(ISender sender) : ControllerBase
{
    [HttpGet("/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        if (ReservedSlugs.Values.Contains(slug))
            return NotFound();

        var result = await sender.Send(new GetSiteBySlugQuery(slug), cancellationToken);
        if (result.Failed)
            return NotFound(new { slug, exists = false });

        return Ok(new { slug = result.Value!.Slug, exists = true, publishState = result.Value.PublishState.ToString() });
    }
}
