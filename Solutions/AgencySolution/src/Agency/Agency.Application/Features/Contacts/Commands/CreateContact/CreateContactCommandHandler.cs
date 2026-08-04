using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using Agency.Domain.Entities;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Commands.CreateContact;

public class CreateContactCommandHandler(IAgencyDbContext db)
    : IRequestHandler<CreateContactCommand, Result<ContactDto>>
{
    public async Task<Result<ContactDto>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = Contact.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Title,
            request.Category,
            request.CompanyId);

        db.Contacts.Add(contact);
        await db.SaveChangesAsync(cancellationToken);

        var dto = new ContactDto(
            contact.Id,
            contact.FirstName,
            contact.LastName,
            contact.Email,
            contact.Phone,
            contact.Title,
            contact.Category,
            contact.CompanyId,
            contact.CreatedAt);

        return Result<ContactDto>.Success(dto);
    }
}
