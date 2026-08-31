namespace TodoApp.Core.Interfaces;

public interface ICategoryService
{
    public async Task<IEnumerable<CategoryService>> GetAllAsync();
    public async Task<CategoryService> GetByIdAsync(Guid id);
    public async Task<CategoryService> CreateAsync(RequestCreateBody request);
    public async Task UpdateAsync(RequestUpdateBody request);
    public async Task DeleteAsync(Guid id);
}