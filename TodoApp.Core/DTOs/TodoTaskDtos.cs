namespace TodoApp.Core.DTOs;

public record TodoTaskDto(Guid Id, string Title, string? Description, bool IsCompleted, Guid? CategoryId);
public record TodoTaskCreateDto(string Title, string? Description, Guid? CategoryId);
public record TodoTaskUpdateDto(string Title, string? Description, bool IsCompleted, Guid? CategoryId);
