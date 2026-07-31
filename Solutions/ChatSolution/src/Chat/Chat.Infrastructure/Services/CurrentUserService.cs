using Chat.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Chat.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? ProfileId
    {
        get
        {
            var value = User?.FindFirstValue("profile_id");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Username =>
        User?.FindFirstValue(ClaimTypes.Name) ?? User?.FindFirstValue("unique_name");

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
