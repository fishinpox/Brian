using Creator.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Commands.ProvisionSite;

public record ProvisionSiteCommand(string RequestedSlug) : IRequest<Result<SiteDto>>;
