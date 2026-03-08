using Microsoft.EntityFrameworkCore;
using PixsyAPI.Auth;
using PixsyAPI.Data;
using PixsyAPI.DTOs;
using PixsyAPI.ErrorHandling;
using PixsyAPI.Models;
using PixsyAPI.Service;
using PixsyAPI.Services.Interfaces;

namespace PixsyAPI.Services.Implementations;

public sealed class AuthService : IAuthService
{
    private readonly PixsyDbContext _db;
    private readonly PasswordService _passwords;
    private readonly IJwtTokenFactory _tokens;

    public AuthService(PixsyDbContext db, PasswordService passwords, IJwtTokenFactory tokens)
    {
        _db = db;
        _passwords = passwords;
        _tokens = tokens;
    }

    public async Task<AuthDTO.AuthResponse> RegisterAsync(AuthDTO.RegisterRequest dto, CancellationToken ct)
    {
        var userName = dto.UserName.Trim();
        var displayName = dto.DisplayName.Trim();
        var email = dto.Email.Trim();

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new BadRequestException("Всички полета са задължителни.");

        if (await _db.Users.AnyAsync(u => u.UserName == userName, ct))
            throw new ConflictException("Потребителското име вече съществува.");

        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new ConflictException("Имейлът вече съществува.");

        var user = new User
        {
            UserName = userName,
            DisplayName = displayName,
            Email = email,
            PasswordHash = Convert.FromBase64String(_passwords.HashPassword(dto.Password))
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var (token, expiresAtUtc) = _tokens.CreateToken(user);
        return new AuthDTO.AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            User = Mappers.ToUserReadDto(user)
        };
    }

    public async Task<AuthDTO.AuthResponse> LoginAsync(AuthDTO.LoginRequest dto, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName, ct);
        if (user == null)
            throw new UnauthorizedException("Невалидно потребителско име или парола.");

        var storedHash = Convert.ToBase64String(user.PasswordHash);
        if (!_passwords.VerifyPassword(dto.Password, storedHash))
            throw new UnauthorizedException("Невалидно потребителско име или парола.");

        var (token, expiresAtUtc) = _tokens.CreateToken(user);
        return new AuthDTO.AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            User = Mappers.ToUserReadDto(user)
        };
    }
}
