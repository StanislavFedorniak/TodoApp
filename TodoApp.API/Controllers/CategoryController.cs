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
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            return Ok(category);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryCreateDto request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CategoryUpdateDto request, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.UpdateAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}