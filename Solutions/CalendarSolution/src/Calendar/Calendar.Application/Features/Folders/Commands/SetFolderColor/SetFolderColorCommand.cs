using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Folders.Commands.SetFolderColor;

public record SetFolderColorCommand(Guid FolderId, string Background, string Text, string Border) : IRequest<Result>;
