using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Commands.UpdateCompany;

public class UpdateCompanyCommandHandler(IAgencyDbContext db)
    : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await db.Companies.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (company is null)
            return Result<CompanyDto>.Failure("Company not found.");

        company.Update(request.Name, request.Category, request.Website);
        await db.SaveChangesAsync(cancellationToken);

        var dto = new CompanyDto(company.Id, company.Name, company.Category, company.Website, company.CreatedAt);

        return Result<CompanyDto>.Success(dto);
    }
}
