using System.Net;
using System.Net.Http.Json;
using TodoApp.Core.DTOs;

namespace TodoApp.Tests;

public class TodoTaskFilterTests : IClassFixture<TodoAppFactory>
{
    private readonly TodoAppFactory _factory;

    public TodoTaskFilterTests(TodoAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_WithCategoryId_ReturnsOnlyTasksOfThatCategory()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var work = await client.CreateCategoryAsync("Work");
        var inWork = await client.CreateTaskAsync("Report", categoryId: work.Id);
        await client.CreateTaskAsync("Milk");

        var result = await client.GetFromJsonAsync<List<TodoTaskDto>>($"/api/todotask?categoryId={work.Id}");

        var only = Assert.Single(result!);
        Assert.Equal(inWork.Id, only.Id);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAll_WithIsCompleted_ReturnsOnlyMatchingTasks(bool isCompleted)
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var open = await client.CreateTaskAsync("Open");
        var done = await client.CreateTaskAsync("Done");
        var update = await client.PutAsJsonAsync($"/api/todotask/{done.Id}", new TodoTaskUpdateDto("Done", null, true, null));
        update.EnsureSuccessStatusCode();

        var result = await client.GetFromJsonAsync<List<TodoTaskDto>>($"/api/todotask?isCompleted={isCompleted}");

        var expected = isCompleted ? done : open;
        var only = Assert.Single(result!);
        Assert.Equal(expected.Id, only.Id);
    }

    [Fact]
    public async Task GetAll_WithSearch_IgnoresCaseAndReturnsMatchingTitles()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var milk = await client.CreateTaskAsync("Buy MILK");
        await client.CreateTaskAsync("Write report");

        var result = await client.GetFromJsonAsync<List<TodoTaskDto>>("/api/todotask?search=milk");

        var only = Assert.Single(result!);
        Assert.Equal(milk.Id, only.Id);
    }

    [Fact]
    public async Task GetAll_WithoutFilters_ReturnsAllOwnTasks()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        await client.CreateTaskAsync("One");
        await client.CreateTaskAsync("Two");

        var result = await client.GetFromJsonAsync<List<TodoTaskDto>>("/api/todotask");

        Assert.Equal(2, result!.Count);
    }

    [Fact]
    public async Task GetAll_WithTooLongSearch_ReturnsBadRequest()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/todotask?search={new string('a', 201)}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithForeignCategoryId_ReturnsEmpty()
    {
        var userA = await _factory.CreateAuthenticatedClientAsync();
        var userB = await _factory.CreateAuthenticatedClientAsync();
        var category = await userA.CreateCategoryAsync();
        await userA.CreateTaskAsync(categoryId: category.Id);

        var result = await userB.GetFromJsonAsync<List<TodoTaskDto>>($"/api/todotask?categoryId={category.Id}");

        Assert.Empty(result!);
    }

    [Theory]
    [InlineData("isCompleted=false")]
    [InlineData("search=Secret")]
    public async Task GetAll_WithAnyFilter_NeverReturnsForeignTasks(string query)
    {
        var userA = await _factory.CreateAuthenticatedClientAsync();
        var userB = await _factory.CreateAuthenticatedClientAsync();
        await userA.CreateTaskAsync("Secret plan");

        var result = await userB.GetFromJsonAsync<List<TodoTaskDto>>($"/api/todotask?{query}");

        Assert.Empty(result!);
    }
}