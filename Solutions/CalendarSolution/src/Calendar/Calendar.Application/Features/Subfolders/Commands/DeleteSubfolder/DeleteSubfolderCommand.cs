using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Subfolders.Commands.DeleteSubfolder;

public record DeleteSubfolderCommand(Guid SubfolderId) : IRequest<Result>;
