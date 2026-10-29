using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Data;

namespace TodoApp.Tests.Helpers;

public class TodoAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("TODOAPP_TESTS_CONNECTION")
        ?? "Server=(localdb)\\mssqllocaldb;Database=TodoAppDb_Tests;Trusted_Connection=True;MultipleActiveResultSets=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
               ["ConnectionStrings:DefaultConnection"] = ConnectionString,
               ["Jwt:Key"] = "integration-tests-only-signing-key-32+chars",
               ["RateLimiting:LoginPermitLimit"] = "1000",
               ["RateLimiting:AccountDeletePermitLimit"] = "1000"
            });
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}