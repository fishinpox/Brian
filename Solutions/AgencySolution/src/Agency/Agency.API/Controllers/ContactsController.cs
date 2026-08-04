using Agency.Application.Features.Contacts.Commands.CreateContact;
using Agency.Application.Features.Contacts.Commands.DeleteContact;
using Agency.Application.Features.Contacts.Commands.UpdateContact;
using Agency.Application.Features.Contacts.Queries.GetContactById;
using Agency.Application.Features.Contacts.Queries.GetContacts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agency.API.Controllers;

[ApiController]
[Route("api/agency/contacts")]
[Authorize(Roles = "AgencyAdmin")]
public class ContactsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetContacts(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContactsQuery(), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetContactById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContactByIdQuery(id), cancellationToken);
        return result.Failed ? NotFound(result.Errors) : Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact([FromBody] CreateContactCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetContactById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateContact(Guid id, [FromBody] UpdateContactCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await sender.Send(command, cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteContactCommand(id), cancellationToken);
        return result.Failed ? BadRequest(result.Errors) : NoContent();
    }
}
