using MediatR;
using Shared.Infrastructure.Common;

namespace Holodex.Application.Features.Holodex.Commands.StoreHolodexCredential;

public record StoreHolodexCredentialCommand(string ApiKey) : IRequest<Result>;
