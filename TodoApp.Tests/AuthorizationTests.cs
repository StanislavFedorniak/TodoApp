using System.Net;

namespace TodoApp.Tests;

public class AuthorizationTests : IClassFixture<TodoAppFactory>
{
    private readonly HttpClient _client;

    public AuthorizationTests(TodoAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/category")]
    [InlineData("/api/todotask")]
    public async Task Get_WithoutToken_ReturnsUnauthorized(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}