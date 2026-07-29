namespace Identity.Application.Common;

public static class SessionPolicy
{
    /// <summary>Sliding refresh-token lifetime: every successful refresh mints a token with a fresh window of this length.</summary>
    public static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);
}
