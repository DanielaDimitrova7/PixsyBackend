using PixsyAPI.DTOs;

namespace PixsyAPI.Services.Interfaces;

public interface IAuthService
{
    Task<AuthDTO.AuthResponse> RegisterAsync(AuthDTO.RegisterRequest dto, CancellationToken ct);
    Task<AuthDTO.AuthResponse> LoginAsync(AuthDTO.LoginRequest dto, CancellationToken ct);
}
