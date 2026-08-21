using Calendar.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Folders.Queries.GetFolderTree;

public record GetFolderTreeQuery : IRequest<Result<FolderTreeDto>>;
