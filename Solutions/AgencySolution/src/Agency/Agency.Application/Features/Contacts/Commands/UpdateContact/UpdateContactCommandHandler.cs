using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Commands.UpdateContact;

public class UpdateContactCommandHandler(IAgencyDbContext db)
    : IRequestHandler<UpdateContactCommand, Result<ContactDto>>
{
    public async Task<Result<ContactDto>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (contact is null)
            return Result<ContactDto>.Failure("Contact not found.");

        contact.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Title,
            request.Category,
            request.CompanyId);

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
