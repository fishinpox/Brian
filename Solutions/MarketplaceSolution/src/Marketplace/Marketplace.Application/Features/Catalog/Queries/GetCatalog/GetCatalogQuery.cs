using MediatR;
using Shared.Infrastructure.Common;

namespace Marketplace.Application.Features.Catalog.Queries.GetCatalog;

public record GetCatalogQuery : IRequest<Result<List<WallpaperItemDto>>>;
