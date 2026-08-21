using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Folders.Commands.CreateFolder;

public record CreateFolderCommand(string Name) : IRequest<Result<FolderDto>>;
