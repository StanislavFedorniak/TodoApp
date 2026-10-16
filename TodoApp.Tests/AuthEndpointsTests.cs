using System.Net.Http.Json;
using TodoApp.Core.DTOs;

namespace TodoApp.Tests;

public class AuthEndpointsTests : IClassFixture<TodoAppFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(TodoAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreatedWithToken()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterDto(HttpClientAuthExtentions.NewEmail(), HttpClientAuthExtentions.DefaultPassword));
    }
}