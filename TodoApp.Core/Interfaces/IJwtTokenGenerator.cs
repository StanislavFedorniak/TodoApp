using TodoApp.Core.DTOs;
using TodoApp.Core.Entities;

namespace TodoApp.Core.Interfaces;

public interface IJwtTokenGenerator
{
    AuthResponseDto Generate(User user);
}