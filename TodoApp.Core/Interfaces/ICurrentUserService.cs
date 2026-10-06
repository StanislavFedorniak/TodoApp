namespace TodoApp.Core.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}