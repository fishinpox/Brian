using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Subfolders.Commands.SetSubfolderVisibility;

public record SetSubfolderVisibilityCommand(Guid SubfolderId, bool IsVisible) : IRequest<Result>;
