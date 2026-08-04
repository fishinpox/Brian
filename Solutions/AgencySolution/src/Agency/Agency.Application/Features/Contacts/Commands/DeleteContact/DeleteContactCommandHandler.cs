using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Contacts.Commands.DeleteContact;

public class DeleteContactCommandHandler(IAgencyDbContext db)
    : IRequestHandler<DeleteContactCommand, Result>
{
    public async Task<Result> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (contact is null)
            return Result.Failure("Contact not found.");

        db.Contacts.Remove(contact);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
