using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Core.Interfaces;

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

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(CancellationToken cancellationToken)
    {
        await _accountService.DeleteAsync(cancellationToken);

        return NoContent();
    }
}