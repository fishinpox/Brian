using MediatR;
using Shared.Infrastructure.Common;

namespace Marketplace.Application.Features.Ownership.Commands.MarkOwnershipApplied;

public record MarkOwnershipAppliedCommand(Guid OwnershipId) : IRequest<Result>;
