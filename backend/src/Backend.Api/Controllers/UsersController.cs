using Backend.Api.Dtos;
using Backend.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPost("block")]
    public async Task<IActionResult> Block([FromBody] BulkActionRequest request)
    {
        await _userService.BlockUsersAsync(request.UserIds);
        return Ok(new { message = "Users blocked" });
    }

    [HttpPost("unblock")]
    public async Task<IActionResult> Unblock([FromBody] BulkActionRequest request)
    {
        await _userService.UnblockUsersAsync(request.UserIds);
        return Ok(new { message = "Users unblocked" });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] BulkActionRequest request)
    {
        await _userService.DeleteUsersAsync(request.UserIds);
        return Ok(new { message = "Users deleted" });
    }

    [HttpDelete("unverified")]
    public async Task<IActionResult> DeleteUnverified([FromBody] BulkActionRequest request)
    {
        await _userService.DeleteUnverifiedUsersAsync(request.UserIds);
        return Ok(new { message = "Unverified users deleted" });
    }
}
