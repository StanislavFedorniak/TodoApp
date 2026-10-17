using System.Net;
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
            new RegisterDto(HttpClientAuthExtensions.NewEmail(), HttpClientAuthExtensions.DefaultPassword));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.False(string.IsNullOrWhiteSpace(body?.Token));
    }

    [Fact]
    public async Task Register_WithTakenEmail_ReturnsConflict()
    {
        var email = HttpClientAuthExtensions.NewEmail();
        await _client.RegisterAsync(email);

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterDto(email, HttpClientAuthExtensions.DefaultPassword));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithShortPassword_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterDto(HttpClientAuthExtensions.NewEmail(), "short"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var email = HttpClientAuthExtensions.NewEmail();
        await _client.RegisterAsync(email);

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto(email, HttpClientAuthExtensions.DefaultPassword));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.False(string.IsNullOrWhiteSpace(body?.Token));
    }

    [Fact]
    public async Task Login_WithWrongPasswordOrUnknownEmail_ReturnsSameUnauthorizedResponse()
    {
        var email = HttpClientAuthExtensions.NewEmail();
        await _client.RegisterAsync(email);

        var wrongPassword = await _client.PostAsJsonAsync(
            "/api/auth/login", new LoginDto(email, "WrongPassw0rd!"));
        var unknownEmail = await _client.PostAsJsonAsync(
            "/api/auth/login", new LoginDto(HttpClientAuthExtensions.NewEmail(), HttpClientAuthExtensions.DefaultPassword));

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unknownEmail.StatusCode);
        Assert.Equal(
            await wrongPassword.Content.ReadAsStringAsync(),
            await unknownEmail.Content.ReadAsStringAsync());
    }
}