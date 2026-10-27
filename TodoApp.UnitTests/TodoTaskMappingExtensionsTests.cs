using TodoApp.Core.DTOs;
using TodoApp.Core.Entities;
using TodoApp.Core.Mappings;

namespace TodoApp.UnitTests;

public class TodoTaskMappingExtensionsTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("2 liters", true)]
    public void ToEntity_CopiesFieldsFromDto(string? description, bool hasCategory)
    {
        Guid? categoryId = hasCategory ? Guid.NewGuid() : null;
        var dto = new TodoTaskCreateDto("Buy milk", description, categoryId);

        var entity = dto.ToEntity();

        Assert.Equal("Buy milk", entity.Title);
        Assert.Equal(description, entity.Description);
        Assert.Equal(categoryId, entity.CategoryId);
    }

    [Fact]
    public void ToEntity_LeavesOwnerAndStatusForService()
    {
        var dto = new TodoTaskCreateDto("Buy milk", null, null);

        var entity = dto.ToEntity();

        Assert.Equal(Guid.Empty, entity.UserId);
        Assert.False(entity.IsCompleted);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToTodoTaskDto_CopiesAllFields(bool isCompleted)
    {
        var entity = new TodoTask
        {
            Title = "Buy milk",
            Description = "2 liters",
            IsCompleted = isCompleted,
            CategoryId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var dto = entity.ToTodoTaskDto();

        var expected = new TodoTaskDto(entity.Id, "Buy milk", "2 liters", isCompleted, entity.CategoryId);
        Assert.Equal(expected, dto);
    }

    [Fact]
    public void MapTo_UpdatesEditableFields_KeepsIdAndOwner()
    {
        var ownerId = Guid.NewGuid();
        var task = new TodoTask
        {
            Title = "Old",
            Description = "Old description",
            IsCompleted = false,
            CategoryId = Guid.NewGuid(),
            UserId = ownerId
        };
        var originalId = task.Id;
        var newCategoryId = Guid.NewGuid();
        var request = new TodoTaskUpdateDto("New", "New description", true, newCategoryId);

        request.MapTo(task);

        Assert.Equal("New", task.Title);
        Assert.Equal("New description", task.Description);
        Assert.True(task.IsCompleted);
        Assert.Equal(newCategoryId, task.CategoryId);
        Assert.Equal(originalId, task.Id);
        Assert.Equal(ownerId, task.UserId);
    }

    [Fact]
    public void MapTo_WithNullOptionalFields_ClearsThem()
    {
        var task = new TodoTask
        {
            Title = "Old",
            Description = "Old description",
            CategoryId = Guid.NewGuid()
        };
        var request = new TodoTaskUpdateDto("New", null, false, null);

        request.MapTo(task);

        Assert.Null(task.Description);
        Assert.Null(task.CategoryId);
    }

    
}