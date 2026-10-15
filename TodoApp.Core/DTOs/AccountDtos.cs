using System.ComponentModel.DataAnnotations;

namespace TodoApp.Core.DTOs;

public record AccountDeleteDto(
    [Required] string Password);