using TodoApp.Core.DTOs;

namespace TodoApp.Core.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(CategoryCreateDto request);
    Task UpdateAsync(Guid id, CategoryUpdateDto request);
    Task DeleteAsync(Guid id);
}