using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Queries.GetContactById;

public class GetContactByIdQueryHandler(IAgencyDbContext db)
    : IRequestHandler<GetContactByIdQuery, Result<ContactDto>>
{
    public async Task<Result<ContactDto>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var contact = await db.Contacts
            .Where(c => c.Id == request.Id)
            .Select(c => new ContactDto(
                c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.Title, c.Category, c.CompanyId, c.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return contact is null
            ? Result<ContactDto>.Failure("Contact not found.")
            : Result<ContactDto>.Success(contact);
    }
}
