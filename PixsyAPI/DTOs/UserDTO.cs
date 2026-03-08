using PixsyAPI.Models;

namespace PixsyAPI.DTOs;

public static class UserDTO
{
    public sealed class UserReadDto
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    public sealed class UpdateMeRequest
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
    }
}
