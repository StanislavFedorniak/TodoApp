using System.ComponentModel.DataAnnotations;

namespace TodoApp.Core.DTOs;

public record TodoTaskDto(Guid Id, string Title, string? Description, bool IsCompleted, Guid? CategoryId);
public record TodoTaskCreateDto(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    Guid? CategoryId);
public record TodoTaskUpdateDto(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    bool IsCompleted,
    Guid? CategoryId);

public record TodoTaskFilterDto(
    Guid? CategoryId = null,
    bool? IsCompleted = null,
    [MaxLength(200)] string? Search = null);
