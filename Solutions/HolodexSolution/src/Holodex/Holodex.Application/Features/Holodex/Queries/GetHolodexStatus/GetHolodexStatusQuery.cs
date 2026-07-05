using MediatR;
using Shared.Infrastructure.Common;

namespace Holodex.Application.Features.Holodex.Queries.GetHolodexStatus;

public record HolodexStatusDto(bool HasCredential, int FollowedCount);

public record GetHolodexStatusQuery : IRequest<Result<HolodexStatusDto>>;
