using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoApp.Core.DTOs;

namespace TodoApp.Tests.Helpers;

public static class HttpClientAuthExtensions
{
    public const string DefaultPassword = "Passw0rd!123";

    public static string NewEmail() => $"user-{Guid.NewGuid():N}@example.com";

    public static async Task<AuthResponseDto> RegisterAsync(this HttpClient client, string email, string password = DefaultPassword)
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(email, password));
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthResponseDto>())!;
    }

    public static async Task<HttpClient> CreateAuthenticatedClientAsync(this TodoAppFactory factory)
    {
        var client = factory.CreateClient();
        var auth = await client.RegisterAsync(NewEmail());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        return client;
    }
}