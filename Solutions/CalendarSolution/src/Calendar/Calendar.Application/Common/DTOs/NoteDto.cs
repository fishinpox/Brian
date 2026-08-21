namespace Calendar.Application.Common.DTOs;

public record NoteDto(Guid Id, string Title, string Content, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
