namespace PixsyAPI.Models
{
    public class Picture
    {
        public int PictureID { get; set; }
        public int UserID { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public List<int> TagsIds { get; set; } = new();
    }
}
