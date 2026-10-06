using Microsoft.AspNetCore.Identity;
using TodoApp.Core.Interfaces;
using TodoApp.Data;
using TodoApp.Core.Entities;
using TodoApp.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using TodoApp.Core.Exceptions;

namespace TodoApp.Services;

public class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Invalid email or password";

    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);

        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);

        if (emailTaken)
        {
            throw new ConflictException("Email is already registered");
        }

        var user = new User { Email = email };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return _tokenGenerator.Generate(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        if (user is null)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        return _tokenGenerator.Generate(user);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
