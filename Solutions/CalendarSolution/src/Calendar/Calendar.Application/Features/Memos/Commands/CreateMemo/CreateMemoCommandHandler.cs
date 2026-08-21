using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using Calendar.Domain.Entities;
using MediatR;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Memos.Commands.CreateMemo;

public class CreateMemoCommandHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<CreateMemoCommand, Result<MemoDto>>
{
    public async Task<Result<MemoDto>> Handle(CreateMemoCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var memo = Memo.Create(currentUser.ProfileId.Value, request.Date, request.Text);
        db.Memos.Add(memo);
        await db.SaveChangesAsync(cancellationToken);

        return Result<MemoDto>.Success(new MemoDto(memo.Id, memo.Date, memo.Text, memo.CreatedAt));
    }
}
