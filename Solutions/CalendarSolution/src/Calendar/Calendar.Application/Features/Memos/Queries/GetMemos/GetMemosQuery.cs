using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Memos.Queries.GetMemos;

public record GetMemosQuery(DateOnly From, DateOnly To) : IRequest<Result<List<MemoDto>>>;
