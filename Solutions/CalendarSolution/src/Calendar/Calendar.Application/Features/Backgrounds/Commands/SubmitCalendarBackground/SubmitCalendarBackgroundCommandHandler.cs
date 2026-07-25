using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events.Calendar;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Backgrounds.Commands.SubmitCalendarBackground;

public class SubmitCalendarBackgroundCommandHandler(
    ICalendarDbContext db,
    ICurrentUserService currentUser,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<SubmitCalendarBackgroundCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SubmitCalendarBackgroundCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var existing = await db.CalendarBackgrounds
            .FirstOrDefaultAsync(b => b.ProfileId == profileId, cancellationToken);

        if (existing is null)
        {
            existing = CalendarBackground.Submit(
                profileId, request.ImageData, request.ContentType, request.FileName);
            db.CalendarBackgrounds.Add(existing);
        }
        else
        {
            existing.Resubmit(request.ImageData, request.ContentType, request.FileName);
        }

        await db.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new CalendarBackgroundSubmittedEvent(
            existing.Id,
            profileId,
            request.ImageData,
            request.ContentType,
            request.FileName,
            request.ImageData.LongLength,
            DateTimeOffset.UtcNow), cancellationToken);

        return Result<Guid>.Success(existing.Id);
    }
}
