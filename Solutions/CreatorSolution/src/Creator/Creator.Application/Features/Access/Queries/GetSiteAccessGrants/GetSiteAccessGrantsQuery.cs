using Creator.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Access.Queries.GetSiteAccessGrants;

public record GetSiteAccessGrantsQuery(Guid SiteId) : IRequest<Result<List<SiteAccessGrantDto>>>;
