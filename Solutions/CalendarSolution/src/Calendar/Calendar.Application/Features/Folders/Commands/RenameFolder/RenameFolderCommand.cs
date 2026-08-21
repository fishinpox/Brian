using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Folders.Commands.RenameFolder;

public record RenameFolderCommand(Guid FolderId, string Name) : IRequest<Result>;
