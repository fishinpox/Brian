using Agency.Application.Common.DTOs;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Queries.GetCompanyById;

public record GetCompanyByIdQuery(Guid Id) : IRequest<Result<CompanyDto>>;
