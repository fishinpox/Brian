using Agency.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Queries.GetContactById;

public record GetContactByIdQuery(Guid Id) : IRequest<Result<ContactDto>>;
