namespace TodoApp.Core.DTOs;

public record CategoryDto(Guid Id, string Name);
public record CategoryCreateDto(string Name);
public record CategoryUpdateDto(string Name);