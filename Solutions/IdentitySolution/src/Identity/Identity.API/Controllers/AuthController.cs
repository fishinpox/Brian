using System.Security.Cryptography;
using Identity.Application.Features.Auth.Commands.GoogleLogin;
using Identity.Application.Features.Auth.Commands.Login;
using Identity.Application.Features.Auth.Commands.RegisterAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(Register), result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        if (result.Failed)
            return Unauthorized(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully." });
    }

    [HttpGet("google/authorize")]
    public IActionResult GoogleAuthorize()
    {
        var state = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        HttpContext.Response.Cookies.Append("oauth_state", state, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10)
        });

        var clientId = config["Google:ClientId"];
        var redirectUri = Uri.EscapeDataString(config["Google:RedirectUri"]!);
        var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email%20profile&state={Uri.EscapeDataString(state)}&access_type=offline&prompt=consent";

        return Redirect(url);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state, CancellationToken cancellationToken)
    {
        var savedState = HttpContext.Request.Cookies["oauth_state"];
        if (savedState is null || savedState != state)
            return BadRequest("Invalid OAuth state.");

        HttpContext.Response.Cookies.Delete("oauth_state");

        var result = await sender.Send(new GoogleLoginCommand(code), cancellationToken);
        if (result.Failed)
            return BadRequest(result.Errors);

        var response = result.Value!;
        if (response.NeedsProfile)
            return Redirect($"/profile-setup.html?token={Uri.EscapeDataString(response.Token)}");

        return Redirect($"https://localhost:7002/calendar.html?token={Uri.EscapeDataString(response.Token)}");
    }
}
