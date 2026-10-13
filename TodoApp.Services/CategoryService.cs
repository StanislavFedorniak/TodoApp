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
    private readonly Guid _userId;
    
    public CategoryService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _userId = currentUser.UserId;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories
            .Where(c => c.UserId == _userId)
            .OrderByDescending(x => x.Id)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return categories;
    }
    
    public async Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .Where(c => c.Id == id && c.UserId == _userId)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            throw new NotFoundException($"Category {id} was not found");
        }

        return category;
    }

    public async Task<CategoryDto> CreateAsync(CategoryCreateDto request, CancellationToken cancellationToken = default)
    {
        var category = request.ToEntity();
        category.UserId = _userId;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.ToCategoryDto();
    }

    public async Task UpdateAsync(Guid id, CategoryUpdateDto request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == _userId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException($"Category {id} was not found");
        }
        
        request.MapTo(category);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == _userId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException($"Category {id} was not found");
        }
        
        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);
    }
}