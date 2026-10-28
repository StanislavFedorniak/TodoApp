using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using TodoApp.API.ExceptionHandling;
using TodoApp.Core.Exceptions;

namespace TodoApp.UnitTests;

public class GlobalExceptionHandlerTests
{
    public static TheoryData<Exception, int> DomainExceptions => new()
    {
        { new NotFoundException("Task was not found"), StatusCodes.Status404NotFound },
        { new ConflictException("Email is already registered"), StatusCodes.Status409Conflict },
        { new UnauthorizedException("Invalid email or password"), StatusCodes.Status401Unauthorized },
        { new ForbiddenException("Password is incorrect"), StatusCodes.Status403Forbidden }
    };

    private static async Task<(bool Handled, int StatusCode, string Body)> HandleAsync(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        return (handled, context.Response.StatusCode, body);
    }

    [Theory]
    [MemberData(nameof(DomainExceptions))]
    public async Task TryHandleAsync_DomainException_ReturnsMappedStatusWithMessage(Exception exception, int expectedStatus)
    {
        var (handled, statusCode, body) = await HandleAsync(exception);

        Assert.True(handled);
        Assert.Equal(expectedStatus, statusCode);

        using var json = JsonDocument.Parse(body);

        Assert.Equal(expectedStatus, json.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(exception.Message, json.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_UnexpectedException_ReturnsGeneric500WithoutLeakingMessage()
    {
        var exception = new InvalidOperationException("Server=prod-db;Password=hunter2");

        var (handled, statusCode, body) = await HandleAsync(exception);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
        Assert.DoesNotContain("hunter2", body);

        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.TryGetProperty("detail", out _));
    }
}