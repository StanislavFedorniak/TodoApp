using TodoApp.Core.DTOs;

namespace TodoApp.Core.Interfaces;

public interface IAccountService
{
    Task DeleteAsync(AccountDeleteDto request, CancellationToken cancellationToken = default);
}