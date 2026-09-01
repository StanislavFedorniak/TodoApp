namespace TodoApp.Core.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetAllAsync();
    Task<CategoryDTO> GetByIdAsync(Guid id);
    Task<CategoryDTO> CreateAsync(CategoryCreateDto request);
    Task UpdateAsync(Guid id, CategoryUpdateDto request);
    Task DeleteAsync(Guid id);
}