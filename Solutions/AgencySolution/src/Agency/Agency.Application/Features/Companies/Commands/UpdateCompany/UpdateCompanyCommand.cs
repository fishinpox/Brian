using Agency.Application.Common.DTOs;
using Agency.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    CompanyCategory Category,
    string? Website) : IRequest<Result<CompanyDto>>;
