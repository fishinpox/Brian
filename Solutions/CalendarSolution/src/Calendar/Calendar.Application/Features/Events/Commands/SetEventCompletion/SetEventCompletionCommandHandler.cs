using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Events.Commands.SetEventCompletion;

public class SetEventCompletionCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetEventCompletionCommand, Result>
{
    public async Task<Result> Handle(SetEventCompletionCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var personalEvent = await db.PersonalEvents
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.PersonalEvent), request.EventId);

        if (personalEvent.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        personalEvent.SetCompletion(request.IsCompleted);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
