using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Commands.DeleteCompany;

public class DeleteCompanyCommandHandler(IAgencyDbContext db)
    : IRequestHandler<DeleteCompanyCommand, Result>
{
    public async Task<Result> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await db.Companies.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (company is null)
            return Result.Failure("Company not found.");

        db.Companies.Remove(company);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
