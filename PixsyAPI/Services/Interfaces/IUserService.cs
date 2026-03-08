using PixsyAPI.DTOs;

namespace PixsyAPI.Services.Interfaces;

public interface IUserService
{
    Task<UserDTO.UserReadDto> GetByIdAsync(int id, CancellationToken ct);
    Task<UserDTO.UserReadDto> UpdateMeAsync(int requesterUserId, UserDTO.UpdateMeRequest dto, CancellationToken ct);
    Task DeleteAsync(int requesterUserId, CancellationToken ct);
}
