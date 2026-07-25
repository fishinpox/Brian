using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;
using YouTube.Application.Common.Interfaces;
using YouTube.Domain.Entities;

namespace YouTube.Application.Features.YouTube.Commands.SaveFollowedChannels;

public class SaveFollowedChannelsCommandHandler(
    IYouTubeDbContext db,
    ICurrentUserService currentUser)
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

        return Result<int>.Success(request.Channels.Count);
    }
}
