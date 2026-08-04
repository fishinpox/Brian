using Agency.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Queries.GetContacts;

public record GetContactsQuery : IRequest<Result<List<ContactDto>>>;
