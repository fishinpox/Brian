using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Folders.Commands.DeleteFolder;

public record DeleteFolderCommand(Guid FolderId) : IRequest<Result>;
