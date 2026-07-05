using MediatR;
using Shared.Infrastructure.Common;

namespace Holodex.Application.Features.Holodex.Queries.SearchHolodexChannels;

public record SearchHolodexChannelsQuery(string SearchTerm) : IRequest<Result<List<HolodexFavoriteDto>>>;
