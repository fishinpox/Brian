using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Events.Commands.MoveEventToFolder;

public record MoveEventToFolderCommand(Guid EventId, Guid? SubfolderId) : IRequest<Result>;
