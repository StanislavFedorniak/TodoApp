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
    private readonly Guid _userId;

    public TodoTaskService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _userId = currentUser.UserId;
    }

    public async Task<IEnumerable<TodoTaskDto>> GetAllAsync(TodoTaskFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks.Where(t => t.UserId == _userId);

        if (filter.CategoryId is { } categoryId)
        {
            query = query.Where(t => t.CategoryId == categoryId);
        }

        if (filter.IsCompleted is { } isCompleted)
        {
            query = query.Where(t => t.IsCompleted == isCompleted);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(t => t.Title.Contains(search));
        }

        var todoTasks = await query
            .OrderByDescending(t => t.Id)
            .Select(t => new TodoTaskDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CategoryId))
            .ToListAsync(cancellationToken);

        return todoTasks;
    }

    public async Task<TodoTaskDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoTask = await _context.Tasks
            .Where(t => t.Id == id && t.UserId == _userId)
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
        await EnsureCategoryBelongsToUserAsync(request.CategoryId, cancellationToken);

        var todoTask = request.ToEntity();
        todoTask.UserId = _userId;

        _context.Tasks.Add(todoTask);
        await _context.SaveChangesAsync(cancellationToken);

        return todoTask.ToTodoTaskDto();
    }

    public async Task UpdateAsync(Guid id, TodoTaskUpdateDto request, CancellationToken cancellationToken = default)
    {
        var todoTask = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == _userId, cancellationToken);

        if (todoTask is null)
        {
            throw new NotFoundException($"Task {id} was not found");
        }

        await EnsureCategoryBelongsToUserAsync(request.CategoryId, cancellationToken);

        request.MapTo(todoTask);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoTask = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == _userId, cancellationToken);

        if (todoTask is null)
        {
            throw new NotFoundException($"Task {id} was not found");
        }
        
        _context.Tasks.Remove(todoTask);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCategoryBelongsToUserAsync(Guid? categoryId, CancellationToken cancellationToken)
    {
        if (categoryId is null)
        {
            return;
        }

        var exists = await _context.Categories
            .AnyAsync(c => c.Id == categoryId && c.UserId == _userId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException($"Category {categoryId} was not found");
        }
    }
}