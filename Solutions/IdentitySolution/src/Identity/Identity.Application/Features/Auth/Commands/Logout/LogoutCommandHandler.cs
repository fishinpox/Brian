using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler(IIdentityDbContext db, ITokenService tokenService)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);

        var session = await db.Sessions.FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);

        // Idempotent: a session that's already revoked or never existed still counts as "logged out".
        if (session is not null && !session.IsRevoked)
        {
            session.Revoke();
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
