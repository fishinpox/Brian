using Calendar.Application.Features.Settings.Commands.UpdateCalendarSettings;
using Calendar.Application.Features.Settings.Queries.GetCalendarSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Controllers;

[ApiController]
[Route("api/calendar/settings")]
[Authorize]
public class CalendarSettingsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetCalendarSettingsQuery(), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCalendarSettingsCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
