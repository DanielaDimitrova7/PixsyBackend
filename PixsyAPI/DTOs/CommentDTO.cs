namespace PixsyAPI.DTOs;

public static class CommentDTO
{
    public sealed class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }

    public sealed class CommentReadDto
    {
        public int PictureCommentID { get; set; }
        public int PictureID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public bool IsOwner { get; set; }
    }
}
