using Agency.Application.Common.DTOs;
using Agency.Domain.Enums;
using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(
    string Name,
    CompanyCategory Category,
    string? Website) : IRequest<Result<CompanyDto>>;
