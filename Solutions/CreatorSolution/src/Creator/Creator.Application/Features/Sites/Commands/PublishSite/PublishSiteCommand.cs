using Creator.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Commands.PublishSite;

public record PublishSiteCommand(Guid SiteId) : IRequest<Result<SiteDto>>;
