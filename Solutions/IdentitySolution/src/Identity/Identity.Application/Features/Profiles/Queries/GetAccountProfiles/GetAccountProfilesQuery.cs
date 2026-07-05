using Identity.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Profiles.Queries.GetAccountProfiles;

public record GetAccountProfilesQuery : IRequest<Result<List<ProfileDto>>>;
