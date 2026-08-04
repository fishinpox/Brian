using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Queries.GetContacts;

public class GetContactsQueryHandler(IAgencyDbContext db)
    : IRequestHandler<GetContactsQuery, Result<List<ContactDto>>>
{
    public async Task<Result<List<ContactDto>>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var contacts = await db.Contacts
            .Select(c => new ContactDto(
                c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.Title, c.Category, c.CompanyId, c.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<ContactDto>>.Success(contacts);
    }
}
