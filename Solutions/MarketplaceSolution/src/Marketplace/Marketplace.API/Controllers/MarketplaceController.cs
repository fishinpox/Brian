using Marketplace.Application.Features.Catalog.Queries.GetCatalog;
using Marketplace.Application.Features.Catalog.Queries.GetOwnedItemFile;
using Marketplace.Application.Features.Ownership.Commands.MarkOwnershipApplied;
using Marketplace.Application.Features.Ownership.Queries.GetPendingOwnerships;
using Marketplace.Application.Features.Purchases.Commands.PurchaseItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/marketplace")]
[Authorize]
public class MarketplaceController(ISender sender) : ControllerBase
{
    [HttpGet("items")]
    public async Task<IActionResult> GetCatalog(CancellationToken ct)
    {
        var result = await sender.Send(new GetCatalogQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("items/{id:guid}/file")]
    public async Task<IActionResult> GetItemFile(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetOwnedItemFileQuery(id), ct);
        return result.Succeeded ? File(result.Value!.ImageData, result.Value.ContentType, result.Value.FileName) : NotFound();
    }

    [HttpPost("items/{id:guid}/purchase")]
    public async Task<IActionResult> PurchaseItem(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new PurchaseItemCommand(id), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("ownership/pending")]
    public async Task<IActionResult> GetPendingOwnerships(CancellationToken ct)
    {
        var result = await sender.Send(new GetPendingOwnershipsQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost("ownership/{id:guid}/mark-applied")]
    public async Task<IActionResult> MarkApplied(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new MarkOwnershipAppliedCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
