using MediatR;
using Shared.Infrastructure.Common;

namespace Marketplace.Application.Features.Catalog.Queries.GetOwnedItemFile;

public record GetOwnedItemFileQuery(Guid ItemId) : IRequest<Result<WallpaperFileDto>>;
