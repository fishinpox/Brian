using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Subfolders.Commands.CreateSubfolder;

public record CreateSubfolderCommand(Guid FolderId, string Name) : IRequest<Result<SubfolderDto>>;
