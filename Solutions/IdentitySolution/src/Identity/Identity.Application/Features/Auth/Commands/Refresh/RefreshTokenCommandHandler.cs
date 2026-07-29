using Identity.Application.Common;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler(IIdentityDbContext db, ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);

        var session = await db.Sessions.FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);
        if (session is null)
            return Result<RefreshTokenResponse>.Failure("Invalid refresh token.");

        if (session.IsRevoked)
        {
            // A revoked token being presented again means either a replay of an already-rotated-out
            // token (possible theft) or a token from a session the user explicitly logged out of.
            // Either way, treat it as compromised and revoke every other active session for this
            // account+profile rather than just rejecting this one request.
            var activeSessions = await db.Sessions
                .Where(s => s.AccountId == session.AccountId && s.ProfileId == session.ProfileId && !s.IsRevoked)
                .ToListAsync(cancellationToken);

            if (activeSessions.Count > 0)
            {
                foreach (var active in activeSessions)
                    active.Revoke();

                await db.SaveChangesAsync(cancellationToken);
            }

            return Result<RefreshTokenResponse>.Failure("Refresh token has already been used. All sessions have been revoked.");
        }

        if (session.ExpiresAt <= DateTimeOffset.UtcNow)
            return Result<RefreshTokenResponse>.Failure("Refresh token has expired.");

        var profile = await db.Profiles
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == session.ProfileId, cancellationToken);

        if (profile is null)
            return Result<RefreshTokenResponse>.Failure("Profile no longer exists.");

        var accessToken = tokenService.GenerateToken(session.AccountId, profile);
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newTokenHash = tokenService.HashRefreshToken(newRefreshToken);

        var newSession = Session.Issue(
            session.AccountId,
            session.ProfileId,
            newTokenHash,
            DateTimeOffset.UtcNow.Add(SessionPolicy.RefreshTokenLifetime),
            request.IpAddress,
            request.UserAgent);

        db.Sessions.Add(newSession);
        session.RotateTo(newSession.Id);

        await db.SaveChangesAsync(cancellationToken);

        var roles = profile.Roles.Select(r => r.Role.ToString()).ToArray();
        return Result<RefreshTokenResponse>.Success(
            new RefreshTokenResponse(session.AccountId, profile.Id, accessToken, newRefreshToken, roles));
    }
}
