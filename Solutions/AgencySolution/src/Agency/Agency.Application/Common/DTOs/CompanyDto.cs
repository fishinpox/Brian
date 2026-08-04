using Agency.Domain.Enums;

namespace Agency.Application.Common.DTOs;

public record CompanyDto(
    Guid Id,
    string Name,
    CompanyCategory Category,
    string? Website,
    DateTimeOffset CreatedAt);
