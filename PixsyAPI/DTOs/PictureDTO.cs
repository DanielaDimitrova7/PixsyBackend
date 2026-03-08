using Microsoft.AspNetCore.Http;

namespace PixsyAPI.DTOs;

public static class PictureDTO
{
    public sealed class UploadPictureDto
    {
        public IFormFile? File { get; set; }
        public List<int> Tags { get; set; } = new();
    }

    public sealed class PictureReadDto
    {
        public int PictureID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public List<int> TagsIds { get; set; } = new();
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
}
