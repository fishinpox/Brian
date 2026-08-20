using Creator.Application.Common.DTOs;
using Creator.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Access.Commands.GrantAccess;

public record GrantAccessCommand(Guid SiteId, Guid GranteeProfileId, SiteAccessLevel AccessLevel)
    : IRequest<Result<SiteAccessGrantDto>>;
