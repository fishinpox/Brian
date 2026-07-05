using Holodex.Application.Common.Events;
using Holodex.Application.Common.Interfaces;
using Holodex.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Holodex.Application.Features.Holodex.Commands.SaveFollowedChannels;

public class SaveFollowedChannelsCommandHandler(
    IHolodexDbContext db,
    ICurrentUserService currentUser,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<SaveFollowedChannelsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(SaveFollowedChannelsCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var existing = await db.FollowedChannels
            .Where(c => c.ProfileId == profileId)
            .ToListAsync(cancellationToken);
        db.FollowedChannels.RemoveRange(existing);

        foreach (var channel in request.Channels)
        {
            db.FollowedChannels.Add(FollowedChannel.Create(
                profileId,
                channel.ChannelId,
                channel.Name,
                channel.EnglishName,
                channel.PhotoUrl));
        }

        await db.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new HolodexSyncRequestedEvent(profileId), cancellationToken);

        return Result<int>.Success(request.Channels.Count);
    }
}
