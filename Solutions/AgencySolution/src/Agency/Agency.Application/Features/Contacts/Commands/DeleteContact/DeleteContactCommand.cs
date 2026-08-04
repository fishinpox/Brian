using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Commands.DeleteContact;

public record DeleteContactCommand(Guid Id) : IRequest<Result>;
