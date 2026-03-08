using Microsoft.AspNetCore.Mvc;
using PixsyAPI.DTOs;
using PixsyAPI.Services.Interfaces;

namespace PixsyAPI.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthDTO.AuthResponse>> Register([FromBody] AuthDTO.RegisterRequest dto, CancellationToken ct)
        => Ok(await _auth.RegisterAsync(dto, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthDTO.AuthResponse>> Login([FromBody] AuthDTO.LoginRequest dto, CancellationToken ct)
        => Ok(await _auth.LoginAsync(dto, ct));
}
