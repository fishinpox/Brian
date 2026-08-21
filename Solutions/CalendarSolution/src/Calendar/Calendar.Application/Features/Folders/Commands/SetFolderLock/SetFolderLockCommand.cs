using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Folders.Commands.SetFolderLock;

public record SetFolderLockCommand(Guid FolderId, bool IsLocked) : IRequest<Result>;
