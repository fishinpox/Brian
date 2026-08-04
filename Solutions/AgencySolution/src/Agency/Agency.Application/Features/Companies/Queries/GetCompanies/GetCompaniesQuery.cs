using Agency.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Queries.GetCompanies;

public record GetCompaniesQuery : IRequest<Result<List<CompanyDto>>>;
