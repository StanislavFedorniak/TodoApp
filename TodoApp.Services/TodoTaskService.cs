using Microsoft.EntityFrameworkCore;
using TodoApp.Core.DTOs;
using TodoApp.Core.Interfaces;
using TodoApp.Data;
using TodoApp.Core.Exceptions;
using TodoApp.Core.Mappings;

namespace TodoApp.Services;

public class TodoTaskService : ITodoTaskService
{
    private readonly AppDbContext _context;

    public TodoTaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoTaskDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var todoTasks = await _context.Tasks
            .OrderByDescending(t => t.Id)
            .Select(t => new TodoTaskDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CategoryId))
            .ToListAsync(cancellationToken);

        return todoTasks;
    }

    public async Task<TodoTaskDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoTask = await _context.Tasks
            .Where(t => t.Id == id)
            .Select(t => new TodoTaskDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CategoryId))
            .FirstOrDefaultAsync(cancellationToken);

        if (todoTask is null)
        {
            throw new NotFoundException($"Task {id} was not found");
        }

        return todoTask;
    }

    public async Task<TodoTaskDto> CreateAsync(TodoTaskCreateDto request, CancellationToken cancellationToken = default)
    {
        var todoTask = request.ToEntity();

        _context.Tasks.Add(todoTask);
        await _context.SaveChangesAsync(cancellationToken);

        return todoTask.ToTodoTaskDto();
    }

    public async Task UpdateAsync(Guid id, TodoTaskUpdateDto request, CancellationToken cancellationToken = default)
    {
        var todoTask = await _context.Tasks.FindAsync([id], cancellationToken);

        if (todoTask is null)
        {
            throw new NotFoundException($"Task {id} was not found");
        }

        request.MapTo(todoTask);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoTask = await _context.Tasks.FindAsync([id], cancellationToken);

        if (todoTask is null)
        {
            throw new NotFoundException($"Task {id} was not found");
        }
        
        _context.Tasks.Remove(todoTask);

        await _context.SaveChangesAsync(cancellationToken);
    }
}