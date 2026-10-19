using System.Net.Http.Json;
using System.Net;
using TodoApp.Core.DTOs;

namespace TodoApp.Tests;

public class DataIsolationTests : IClassFixture<TodoAppFactory>
{
    private readonly TodoAppFactory _factory;

    public DataIsolationTests(TodoAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyOwnTasksAndCategories()
    {
        var userA = await _factory.CreateAuthenticatedClientAsync();
        var userB = await _factory.CreateAuthenticatedClientAsync();
        var category = await userA.CreateCategoryAsync();
        var task = await userA.CreateTaskAsync(categoryId: category.Id);

        var tasksOfB = await userB.GetFromJsonAsync<List<TodoTaskDto>>("/api/todotask");
        var categoriesOfB = await userB.GetFromJsonAsync<List<CategoryDto>>("/api/category");
        var tasksOfA = await userA.GetFromJsonAsync<List<TodoTaskDto>>("/api/todotask");
        
        Assert.Empty(tasksOfB!);
        Assert.Empty(categoriesOfB!);
        Assert.Contains(tasksOfA!, t => t.Id == task.Id);
    }

    [Fact]
    public async Task TaskEndpoints_ForForeignTask_ReturnNotFound()
    {
        var userA = await _factory.CreateAuthenticatedClientAsync();
        var userB = await _factory.CreateAuthenticatedClientAsync();
        var task = await userA.CreateTaskAsync();
        var url = $"/api/todotask/{task.Id}";

        var get = await userB.GetAsync(url);
        var put = await userB.PutAsJsonAsync(url, new TodoTaskUpdateDto("Hacked", null, true, null));
        var delete = await userB.DeleteAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);

        var untouched = await userA.GetFromJsonAsync<TodoTaskDto>(url);
        Assert.Equal("Buy milk", untouched!.Title);
        Assert.False(untouched.IsCompleted);
    }

    [Fact]
    public async Task CategoryEndpoints_ForForeignCategory_ReturnNotFound()
    {
        var userA = await _factory.CreateAuthenticatedClientAsync();
        var userB = await _factory.CreateAuthenticatedClientAsync();
        var category = await userA.CreateCategoryAsync();
        var url = $"/api/category/{category.Id}";

        var get = await userB.GetAsync(url);
        var put = await userB.PutAsJsonAsync(url, new CategoryUpdateDto("Hacked"));
        var delete = await userB.DeleteAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);

        var untouched = await userA.GetFromJsonAsync<CategoryDto>(url);
        Assert.Equal("Work", untouched!.Name);
    }

    [Fact]
    public async Task CreateTask_WithForeignCategory_ReturnsNotFound()
    {
        var userA = await _factory.CreateAuthenticatedClientAsync();
        var userB = await _factory.CreateAuthenticatedClientAsync();
        var foreignCategory = await userA.CreateCategoryAsync();

        var response = await userB.PostAsJsonAsync(
            "/api/todotask", new TodoTaskCreateDto("Sneaky", null, foreignCategory.Id));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var tasksOfB = await userB.GetFromJsonAsync<List<TodoTaskDto>>("/api/todotask");
        Assert.Empty(tasksOfB!);
    }

    [Fact]
    public async Task UpdateTask_WithForeignCategory_ReturnsNotFoundAndKeepsTask()
    {
        var userA = await _factory.CreateAuthenticatedClientAsync();
        var userB = await _factory.CreateAuthenticatedClientAsync();
        var foreignCategory = await userA.CreateCategoryAsync();
        var ownTask = await userB.CreateTaskAsync();
        var url = $"/api/todotask/{ownTask.Id}";

        var response = await userB.PutAsJsonAsync(
            url, new TodoTaskUpdateDto("Sneaky", null, true, foreignCategory.Id));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var untouched = await userB.GetFromJsonAsync<TodoTaskDto>(url);
        Assert.Equal("Buy milk", untouched!.Title);
        Assert.False(untouched.IsCompleted);
        Assert.Null(untouched.CategoryId);
    }
}