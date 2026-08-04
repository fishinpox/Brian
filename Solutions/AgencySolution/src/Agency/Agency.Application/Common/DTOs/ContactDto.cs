using Agency.Domain.Enums;

namespace Agency.Application.Common.DTOs;

public record ContactDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Title,
    ContactCategory Category,
    Guid? CompanyId,
    DateTimeOffset CreatedAt);
