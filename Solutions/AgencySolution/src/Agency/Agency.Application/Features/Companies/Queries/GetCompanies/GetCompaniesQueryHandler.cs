using Agency.Application.Common.DTOs;
using Agency.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Queries.GetCompanies;

public class GetCompaniesQueryHandler(IAgencyDbContext db)
    : IRequestHandler<GetCompaniesQuery, Result<List<CompanyDto>>>
{
    public async Task<Result<List<CompanyDto>>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await db.Companies
            .Select(c => new CompanyDto(c.Id, c.Name, c.Category, c.Website, c.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<CompanyDto>>.Success(companies);
    }
}
