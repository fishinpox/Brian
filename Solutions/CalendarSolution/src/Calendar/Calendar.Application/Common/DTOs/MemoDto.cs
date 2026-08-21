namespace Calendar.Application.Common.DTOs;

public record MemoDto(Guid Id, DateOnly Date, string Text, DateTimeOffset CreatedAt);
