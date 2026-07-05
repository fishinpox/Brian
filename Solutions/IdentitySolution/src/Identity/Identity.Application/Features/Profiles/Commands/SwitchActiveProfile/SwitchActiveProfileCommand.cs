using MediatR;
using Shared.Infrastructure.Common;

namespace Identity.Application.Features.Profiles.Commands.SwitchActiveProfile;

public record SwitchActiveProfileCommand(Guid ProfileId) : IRequest<Result<SwitchProfileResponse>>;
