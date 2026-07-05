using Holodex.Application.Common.Events;
using Holodex.Application.Common.Interfaces;
using MassTransit;
using MediatR;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Holodex.Application.Features.Holodex.Commands.TriggerHolodexSync;

public class TriggerHolodexSyncCommandHandler(
    ICurrentUserService currentUser,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<TriggerHolodexSyncCommand, Result>
{
    public async Task<Result> Handle(TriggerHolodexSyncCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        await publishEndpoint.Publish(new HolodexSyncRequestedEvent(currentUser.ProfileId.Value), cancellationToken);

        return Result.Success();
    }
}
