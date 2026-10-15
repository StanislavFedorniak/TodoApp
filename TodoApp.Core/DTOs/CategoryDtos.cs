using System.ComponentModel.DataAnnotations;

namespace TodoApp.Core.DTOs;

public record CategoryDto(Guid Id, string Name);
public record CategoryCreateDto([Required, MaxLength(100)] string Name);
public record CategoryUpdateDto([Required, MaxLength(100)] string Name);