using Calendar.Application.Common.DTOs;
using Calendar.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;
using Shared.Infrastructure.Common.Exceptions;

namespace Calendar.Application.Features.Memos.Queries.GetMemos;

public class GetMemosQueryHandler(ICalendarDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMemosQuery, Result<List<MemoDto>>>
{
    public async Task<Result<List<MemoDto>>> Handle(GetMemosQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.ProfileId is null)
            throw new ForbiddenAccessException();

        var profileId = currentUser.ProfileId.Value;

        var memos = await db.Memos
            .Where(m => m.ProfileId == profileId && m.Date >= request.From && m.Date <= request.To)
            .OrderBy(m => m.Date).ThenBy(m => m.CreatedAt)
            .Select(m => new MemoDto(m.Id, m.Date, m.Text, m.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<MemoDto>>.Success(memos);
    }
}
