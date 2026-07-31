using MediatR;
using Shared.Infrastructure.Common;

namespace Chat.Application.Features.Accounts.Commands.EnsureChatAccount;

public record EnsureChatAccountCommand : IRequest<Result<EnsureChatAccountResponse>>;
