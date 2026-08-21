using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Memos.Commands.DeleteMemo;

public record DeleteMemoCommand(Guid MemoId) : IRequest<Result>;
