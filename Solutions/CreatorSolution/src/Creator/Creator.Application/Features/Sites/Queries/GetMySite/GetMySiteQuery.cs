using Creator.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Sites.Queries.GetMySite;

public record GetMySiteQuery : IRequest<Result<SiteDto>>;
