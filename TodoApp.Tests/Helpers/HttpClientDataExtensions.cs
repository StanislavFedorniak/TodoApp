using System.Net.Http.Json;
using TodoApp.Core.DTOs;

namespace TodoApp.Tests.Helpers;

public static class HttpClientDataExtensions
{
    public static async Task<CategoryDto> CreateCategoryAsync(this HttpClient client, string name = "Work")
    {
        var response = await client.PostAsJsonAsync("/api/category", new CategoryCreateDto(name));
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }

    public static async Task<TodoTaskDto> CreateTaskAsync(
        this HttpClient client, string title = "Buy milk", Guid? categoryId = null)
    {
        var response = await client.PostAsJsonAsync("/api/todotask", new TodoTaskCreateDto(title, null, categoryId));
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<TodoTaskDto>())!;
    }
}