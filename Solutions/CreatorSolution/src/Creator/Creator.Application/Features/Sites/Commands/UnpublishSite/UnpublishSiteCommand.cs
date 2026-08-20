using Creator.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Commands.UnpublishSite;

public record UnpublishSiteCommand(Guid SiteId) : IRequest<Result<SiteDto>>;
