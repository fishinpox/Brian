using Creator.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Queries.GetSiteBySlug;

public record GetSiteBySlugQuery(string Slug) : IRequest<Result<PublicSiteDto>>;
