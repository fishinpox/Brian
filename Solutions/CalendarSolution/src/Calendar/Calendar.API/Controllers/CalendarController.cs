using Calendar.Application.Features.Backgrounds.Commands.SubmitCalendarBackground;
using Calendar.Application.Features.Backgrounds.Queries.GetCalendarBackground;
using Calendar.Application.Features.Calendar.Queries.GetCalendarView;
using Calendar.Application.Features.Events.Commands.CreatePersonalEvent;
using Calendar.Application.Features.Events.Commands.DeletePersonalEvent;
using Calendar.Application.Features.Events.Commands.UpdatePersonalEvent;
using Calendar.Application.Features.Reminders.Commands.DismissReminder;
using Calendar.Application.Features.Reminders.Commands.SetReminder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.API.Controllers;

[ApiController]
[Route("api/calendar")]
[Authorize]
public class CalendarController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCalendarView(
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetCalendarViewQuery(from, to), ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent([FromBody] CreatePersonalEventCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Succeeded ? CreatedAtAction(nameof(GetCalendarView), new { }, result.Value) : BadRequest(result.Errors);
    }

    [HttpPut("events/{id:guid}")]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdatePersonalEventCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { EventId = id }, ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("events/{id:guid}")]
    public async Task<IActionResult> DeleteEvent(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeletePersonalEventCommand(id), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("events/{eventId:guid}/reminders")]
    public async Task<IActionResult> SetReminder(Guid eventId, [FromBody] SetReminderCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { PersonalEventId = eventId }, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpDelete("reminders/{reminderId:guid}")]
    public async Task<IActionResult> DismissReminder(Guid reminderId, CancellationToken ct)
    {
        var result = await sender.Send(new DismissReminderCommand(reminderId), ct);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPut("background")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> SubmitBackground([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0)
            return BadRequest(new[] { "File is required." });

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);

        var command = new SubmitCalendarBackgroundCommand(stream.ToArray(), file.ContentType, file.FileName);
        var result = await sender.Send(command, ct);
        return result.Succeeded ? Accepted() : BadRequest(result.Errors);
    }

    [HttpGet("background")]
    public async Task<IActionResult> GetBackground(CancellationToken ct)
    {
        var result = await sender.Send(new GetCalendarBackgroundQuery(), ct);
        return result.Succeeded ? File(result.Value!.ImageData, result.Value.ContentType) : NotFound();
    }
}
