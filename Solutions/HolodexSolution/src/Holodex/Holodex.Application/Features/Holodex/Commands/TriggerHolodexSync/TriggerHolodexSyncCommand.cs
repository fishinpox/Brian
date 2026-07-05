using MediatR;
using Shared.Infrastructure.Common;

namespace Holodex.Application.Features.Holodex.Commands.TriggerHolodexSync;

public record TriggerHolodexSyncCommand : IRequest<Result>;
