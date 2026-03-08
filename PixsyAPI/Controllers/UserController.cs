using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixsyAPI.DTOs;
using PixsyAPI.Services.Interfaces;
using PixsyAPI.Services.Security;

namespace PixsyAPI.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _users;
    public UserController(IUserService users) => _users = users;

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDTO.UserReadDto>> GetById(int id, CancellationToken ct)
        => Ok(await _users.GetByIdAsync(id, ct));

    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<UserDTO.UserReadDto>> UpdateMe([FromBody] UserDTO.UpdateMeRequest dto, CancellationToken ct)
        => Ok(await _users.UpdateMeAsync(User.GetUserIdOrThrow(), dto, ct));

    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteMe(CancellationToken ct)
    {
        await _users.DeleteAsync(User.GetUserIdOrThrow(), ct);
        return NoContent();
    }
}
