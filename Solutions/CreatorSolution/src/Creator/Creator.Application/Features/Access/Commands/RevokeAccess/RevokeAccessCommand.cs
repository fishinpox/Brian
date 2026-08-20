using MediatR;
using Shared.Infrastructure.Common;

namespace Creator.Application.Features.Access.Commands.RevokeAccess;

public record RevokeAccessCommand(Guid SiteId, Guid GranteeProfileId) : IRequest<Result>;
