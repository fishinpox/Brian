using MediatR;
using Shared.Infrastructure.Common;

namespace Marketplace.Application.Features.Ownership.Queries.GetPendingOwnerships;

public record GetPendingOwnershipsQuery : IRequest<Result<List<PendingOwnershipDto>>>;
