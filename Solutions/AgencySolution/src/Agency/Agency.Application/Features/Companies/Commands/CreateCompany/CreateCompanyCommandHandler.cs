using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using Agency.Domain.Entities;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler(IAgencyDbContext db)
    : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = Company.Create(request.Name, request.Category, request.Website);

        db.Companies.Add(company);
        await db.SaveChangesAsync(cancellationToken);

        var dto = new CompanyDto(company.Id, company.Name, company.Category, company.Website, company.CreatedAt);

        return Result<CompanyDto>.Success(dto);
    }
}
