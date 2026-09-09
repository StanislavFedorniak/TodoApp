using Microsoft.AspNetCore.Mvc;
using TodoApp.Core.DTOs;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Exceptions;

namespace TodoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
        {
            return NotFound();
        }
        
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryCreateDto request)
    {
        var result = await _categoryService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CategoryUpdateDto request)
    {
        try
        {
            await _categoryService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}