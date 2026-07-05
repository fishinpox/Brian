namespace Identity.Application.Common.DTOs;

public record ProfileDto(
    Guid Id,
    Guid AccountId,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    bool IsPublic,
    string[] Roles,
    DateTimeOffset CreatedAt);
