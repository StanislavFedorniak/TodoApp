using System.ComponentModel.DataAnnotations;

namespace TodoApp.Core.DTOs;

public record RegisterDto(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt
);