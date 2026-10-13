namespace TodoApp.Core.Interfaces;

public interface IAccountService
{
    Task DeleteAsync(CancellationToken cancellationToken = default);
}