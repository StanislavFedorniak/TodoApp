using System.Net;
using System.Net.Http.Json;
using TodoApp.Core.DTOs;

namespace TodoApp.Tests;

public class RateLimitingTests : IClassFixture<LowRateLimitFactory>
{
    private readonly LowRateLimitFactory _factory;

    public RateLimitingTests(LowRateLimitFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ExceedingLimit_ReturnsTooManyRequests()
    {
        var client = _factory.CreateClient();
        var request = new LoginDto(HttpClientAuthExtensions.NewEmail(), "WrongPassword1!");

        var first = await client.PostAsJsonAsync("/api/auth/login", request);
        var second = await client.PostAsJsonAsync("/api/auth/login", request);
        var third = await client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, first.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, second.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
        Assert.NotNull(third.Headers.RetryAfter);
    }

    [Fact]
    public async Task Delete_ExceedingLimit_BlocksOnlyThatUser()
    {
        var blocked = await _factory.CreateAuthenticatedClientAsync();
        var other = await _factory.CreateAuthenticatedClientAsync();
        var wrong = new AccountDeleteDto("WrongPassword");

        var first = await blocked.PostAsJsonAsync("/api/account/delete", wrong);
        var second = await blocked.PostAsJsonAsync("/api/account/delete", wrong);
        var third = await blocked.PostAsJsonAsync("/api/account/delete", wrong);
        var otherUser = await other.PostAsJsonAsync("/api/account/delete", wrong);

        Assert.Equal(HttpStatusCode.Forbidden, first.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, second.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, otherUser.StatusCode);
    }
}