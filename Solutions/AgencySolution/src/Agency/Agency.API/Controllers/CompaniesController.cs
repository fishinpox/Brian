using Agency.Application.Features.Companies.Commands.CreateCompany;
using Agency.Application.Features.Companies.Commands.DeleteCompany;
using Agency.Application.Features.Companies.Commands.UpdateCompany;
using Agency.Application.Features.Companies.Queries.GetCompanies;
using Agency.Application.Features.Companies.Queries.GetCompanyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agency.API.Controllers;

[ApiController]
[Route("api/agency/companies")]
[Authorize(Roles = "AgencyAdmin")]
public class CompaniesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCompanies(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCompaniesQuery(), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCompanyById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCompanyByIdQuery(id), cancellationToken);
        return result.Failed ? NotFound(result.Errors) : Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetCompanyById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await sender.Send(command, cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCompanyCommand(id), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : NoContent();
    }
}
