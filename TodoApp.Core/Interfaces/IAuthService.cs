using TodoApp.Core.DTOs;

namespace TodoApp.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto request, CancellationToken cancellationToken = default);
}