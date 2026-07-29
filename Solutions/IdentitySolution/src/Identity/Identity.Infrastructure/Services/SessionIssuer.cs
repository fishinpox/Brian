using Identity.Application.Common;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Services;

public class SessionIssuer(IIdentityDbContext db, ITokenService tokenService) : ISessionIssuer
{
    public async Task<(string AccessToken, string RefreshToken)> IssueAsync(
        Guid accountId, Profile profile, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var accessToken = tokenService.GenerateToken(accountId, profile);
        var refreshToken = tokenService.GenerateRefreshToken();
        var tokenHash = tokenService.HashRefreshToken(refreshToken);

        var session = Session.Issue(
            accountId,
            profile.Id,
            tokenHash,
            DateTimeOffset.UtcNow.Add(SessionPolicy.RefreshTokenLifetime),
            ipAddress,
            userAgent);

        db.Sessions.Add(session);
        await db.SaveChangesAsync(cancellationToken);

        return (accessToken, refreshToken);
    }
}
