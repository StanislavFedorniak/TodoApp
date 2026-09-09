using TodoApp.Core.DTOs;

namespace TodoApp.Core.Interfaces;

public interface ITodoTaskService
{
    Task<IEnumerable<TodoTaskDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TodoTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TodoTaskDto> CreateAsync(TodoTaskCreateDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, TodoTaskUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}