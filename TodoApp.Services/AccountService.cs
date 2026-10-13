using Microsoft.EntityFrameworkCore;
using TodoApp.Core.Exceptions;
using TodoApp.Core.Interfaces;
using TodoApp.Data;

namespace TodoApp.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly Guid _userId;

    public AccountService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _userId = currentUser.UserId;
    }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        await _context.Tasks
            .Where(t => t.UserId == _userId)
            .ExecuteDeleteAsync(cancellationToken);

        var deletedUsers = await _context.Users
            .Where(u => u.Id == _userId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedUsers == 0)
        {
            throw new NotFoundException("Account was not found");
        }

        await transaction.CommitAsync(cancellationToken);
    }
}