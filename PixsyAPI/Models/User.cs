namespace PixsyAPI.Models
{
    public class User
    {
        public int UserID { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<int> BoardsIds { get; set; } = new();
        public List<int> UploadsIds { get; set; } = new();
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    }

    public enum UserRole
    {
        User,
        Admin
    }
}
