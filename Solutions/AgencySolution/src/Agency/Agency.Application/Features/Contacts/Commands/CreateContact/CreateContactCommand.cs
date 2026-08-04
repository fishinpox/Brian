using Agency.Application.Common.DTOs;
using Agency.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Commands.CreateContact;

public record CreateContactCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Title,
    ContactCategory Category,
    Guid? CompanyId) : IRequest<Result<ContactDto>>;
