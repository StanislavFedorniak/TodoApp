using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Core.Interfaces;
using TodoApp.Core.DTOs;

namespace TodoApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("delete")]
    public async Task<ActionResult> DeleteAsync(AccountDeleteDto request, CancellationToken cancellationToken)
    {
        await _accountService.DeleteAsync(request, cancellationToken);

        return NoContent();
    }
}