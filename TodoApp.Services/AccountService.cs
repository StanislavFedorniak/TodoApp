using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Core.Exceptions;
using TodoApp.Core.Interfaces;
using TodoApp.Data;
using TodoApp.Core.Entities;
using TodoApp.Core.DTOs;

namespace TodoApp.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly Guid _userId;

    public AccountService(AppDbContext context, IPasswordHasher<User> passwordHasher, ICurrentUserService currentUser)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _userId = currentUser.UserId;
    }

    public async Task DeleteAsync(AccountDeleteDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == _userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("Account was not found");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new ForbiddenException("Password is incorrect");
        }

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