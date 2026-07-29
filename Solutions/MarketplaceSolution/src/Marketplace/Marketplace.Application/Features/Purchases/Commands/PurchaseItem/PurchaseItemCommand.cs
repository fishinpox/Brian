using MediatR;
using Shared.Infrastructure.Common;

namespace Marketplace.Application.Features.Purchases.Commands.PurchaseItem;

public record PurchaseItemCommand(Guid ItemId) : IRequest<Result<PurchaseItemResponse>>;
