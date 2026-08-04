using MediatR;
using Shared.Infrastructure.Common;

namespace Agency.Application.Features.Companies.Commands.DeleteCompany;

public record DeleteCompanyCommand(Guid Id) : IRequest<Result>;
