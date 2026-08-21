using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Memos.Commands.CreateMemo;

public record CreateMemoCommand(DateOnly Date, string Text) : IRequest<Result<MemoDto>>;
