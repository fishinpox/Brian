using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;
using YouTube.Application.Common.Interfaces;
using YouTube.Domain.Entities;

namespace YouTube.Application.Features.YouTube.Commands.StoreYouTubeCredential;

public class StoreYouTubeCredentialCommandHandler(
    IYouTubeDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<StoreYouTubeCredentialCommand, Result>
{
    public async Task<Result> Handle(StoreYouTubeCredentialCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;
        var encryptedValue = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.ApiKey));

        var existing = await db.ExternalCredentials
            .FirstOrDefaultAsync(c => c.ProfileId == profileId, cancellationToken);

        if (existing is not null)
        {
            existing.EncryptedValue = encryptedValue;
            existing.ExpiresAt = null;
        }
        else
        {
            db.ExternalCredentials.Add(ExternalCredential.Create(profileId, encryptedValue));
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
