using Holodex.Application.Common.Interfaces;
using Holodex.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Holodex.Application.Features.Holodex.Commands.StoreHolodexCredential;

public class StoreHolodexCredentialCommandHandler(
    IHolodexDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<StoreHolodexCredentialCommand, Result>
{
    public async Task<Result> Handle(StoreHolodexCredentialCommand request, CancellationToken cancellationToken)
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
            var credential = ExternalCredential.Create(profileId, encryptedValue);
            db.ExternalCredentials.Add(credential);
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
