using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Memos.Commands.DeleteMemo;

public class DeleteMemoCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteMemoCommand, Result>
{
    public async Task<Result> Handle(DeleteMemoCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var memo = await db.Memos.FirstOrDefaultAsync(m => m.Id == request.MemoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Memo), request.MemoId);

        if (memo.ProfileId != currentUser.ProfileId.Value)
            throw new ForbiddenAccessException();

        db.Memos.Remove(memo);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
