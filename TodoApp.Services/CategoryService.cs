using Microsoft.EntityFrameworkCore;
using TodoApp.Core.Interfaces;
using TodoApp.Data;
using TodoApp.Core.DTOs;
using TodoApp.Core.Mappings;
using TodoApp.Core.Exceptions;

namespace TodoApp.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    
    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _context.Categories
            .OrderByDescending(x => x.Id)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return response;
    }
    
    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryDto> CreateAsync(CategoryCreateDto request, CancellationToken cancellationToken = default)
    {
        var entity = request.ToEntity();

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToCategoryDto();
    }

    public async Task UpdateAsync(Guid id, CategoryUpdateDto request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            throw new NotFoundException($"Category {id} was not found");
        }
        
        request.MapTo(category);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            throw new NotFoundException($"Category {id} was not found");
        }
        
        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);
    }
}