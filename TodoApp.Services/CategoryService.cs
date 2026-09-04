using Microsoft.EntityFrameworkCore;
using TodoApp.Core.Interfaces;
using TodoApp.Data;
using TodoApp.Core.DTOs;
using TodoApp.Core.Entities;

namespace TodoApp.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    
    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var response = await _context.Categories
            .OrderByDescending(x => x.Id)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync();

        return response;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .FirstOrDefaultAsync();
    }

    public Task<CategoryDto> CreateAsync(CategoryCreateDto request)
    {
        
    }

    private CategoryDto toCategoryDto(CategoryCreateDto request)
    {
        return new CategoryDto()
    }
}