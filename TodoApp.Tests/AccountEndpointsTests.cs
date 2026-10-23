using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoApp.Core.DTOs;

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
        var client = _factory.CreateClient();
        var email = HttpClientAuthExtensions.NewEmail();
        var auth = await client.RegisterAsync(email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        var response = await client.PostAsJsonAsync(Url, new AccountDeleteDto("WrongPassword"));
        var login = await client.PostAsJsonAsync(
            "/api/auth/login", new LoginDto(email, HttpClientAuthExtensions.DefaultPassword));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task Delete_WithCorrectPassword_ReturnsNoContentAndBlocksLogin()
    {
        var client = _factory.CreateClient();
        var email = HttpClientAuthExtensions.NewEmail();
        var auth = await client.RegisterAsync(email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        var category = await client.CreateCategoryAsync();
        await client.CreateTaskAsync(categoryId: category.Id);

        var delete = await client.PostAsJsonAsync(
            Url, new AccountDeleteDto(HttpClientAuthExtensions.DefaultPassword));
        var login = await client.PostAsJsonAsync(
            "/api/auth/login", new LoginDto(email, HttpClientAuthExtensions.DefaultPassword));
    
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }
}