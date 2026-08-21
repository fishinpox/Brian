using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Folders.Commands.SetFolderVisibility;

public record SetFolderVisibilityCommand(Guid FolderId, bool IsVisible) : IRequest<Result>;
