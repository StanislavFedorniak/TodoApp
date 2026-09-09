using TodoApp.Core.DTOs;
using TodoApp.Core.Entities;

namespace TodoApp.Core.Mappings;

public static class TodoTaskMappingExtensions
{
    public static TodoTask ToEntity(this TodoTaskCreateDto request)
    {
        return new TodoTask
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId
        };
    }

    public static TodoTaskDto ToTodoTaskDto(this TodoTask entity)
    {
        return new TodoTaskDto(
                entity.Id, 
                entity.Title, 
                entity.Description, 
                entity.IsCompleted, 
                entity.CategoryId);
    }

    public static void MapTo(this TodoTaskUpdateDto request, TodoTask task)
    {
        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.CategoryId = request.CategoryId;
    }
}