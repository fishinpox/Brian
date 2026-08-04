using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Queries.GetCompanyById;

public class GetCompanyByIdQueryHandler(IAgencyDbContext db)
    : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await db.Companies
            .Where(c => c.Id == request.Id)
            .Select(c => new CompanyDto(c.Id, c.Name, c.Category, c.Website, c.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return company is null
            ? Result<CompanyDto>.Failure("Company not found.")
            : Result<CompanyDto>.Success(company);
    }
}
