using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Subfolders.Commands.RenameSubfolder;

public record RenameSubfolderCommand(Guid SubfolderId, string Name) : IRequest<Result>;
