using Identity.Domain.Entities;

namespace Identity.Application.Common.Interfaces;

public interface ISessionIssuer
{
    /// <summary>Mints an access token plus a new, persisted refresh-token Session for a full (profile-bound) login.</summary>
    Task<(string AccessToken, string RefreshToken)> IssueAsync(
        Guid accountId, Profile profile, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
}
