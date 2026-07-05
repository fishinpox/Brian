using Calendar.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Calendar.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? ProfileId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst("profile_id");
            return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
