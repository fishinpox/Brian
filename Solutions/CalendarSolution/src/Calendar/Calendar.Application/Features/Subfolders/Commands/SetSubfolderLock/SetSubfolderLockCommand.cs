using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Subfolders.Commands.SetSubfolderLock;

public record SetSubfolderLockCommand(Guid SubfolderId, bool IsLocked) : IRequest<Result>;
