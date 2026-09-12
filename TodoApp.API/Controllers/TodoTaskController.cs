using Microsoft.AspNetCore.Mvc;
using TodoApp.Core.DTOs;
using TodoApp.Core.Exceptions;
using TodoApp.Core.Interfaces;

namespace TodoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoTaskController : ControllerBase
{
    private readonly ITodoTaskService _todoTaskService;

    public TodoTaskController(ITodoTaskService todoTaskService)
    {
        _todoTaskService = todoTaskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoTaskDto>>> GetAll(CancellationToken cancellationToken)
    {
        var todoTasks = await _todoTaskService.GetAllAsync(cancellationToken);

        return Ok(todoTasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoTaskDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var todoTask = await _todoTaskService.GetByIdAsync(id, cancellationToken);
            return Ok(todoTask);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<TodoTaskDto>> Create(TodoTaskCreateDto request, CancellationToken cancellationToken)
    {
        var todoTask = await _todoTaskService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new {id = todoTask.Id}, todoTask);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, TodoTaskUpdateDto request, CancellationToken cancellationToken)
    {
        try
        {
            await _todoTaskService.UpdateAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e) // Changed from Exception e to NotFoundException e
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _todoTaskService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
