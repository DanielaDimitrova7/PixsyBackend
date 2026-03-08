namespace PixsyAPI.Models
{
    public class PictureComment
    {
        public int PictureCommentID { get; set; }
        public int PictureID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
