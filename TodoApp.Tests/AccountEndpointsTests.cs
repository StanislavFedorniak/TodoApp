using System.Net;
using System.Net.Http.Json;

namespace TodoApp.Tests;

public class AccountEndpointsTests : IClassFixture<TodoAppFactory>
{
    private const string Url = "/api/account/delete";

    private readonly TodoAppFactory _factory;

    public AccountEndpointsTests(TodoAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Delete_WithoutPassword_ReturnsBadRequest()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync(Url, new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithWrongPassword_ReturnsForbiddenAndKeepsAccount()
    {
        
    }
}