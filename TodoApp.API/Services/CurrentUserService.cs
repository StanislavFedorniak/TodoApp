using Microsoft.IdentityModel.JsonWebTokens;
using TodoApp.Core.Exceptions;
using TodoApp.Core.Interfaces;

namespace TodoApp.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return Guid.TryParse(value, out var userId)
                ? userId
                : throw new UnauthorizedException("User is not authenticated");
        }
    }
}