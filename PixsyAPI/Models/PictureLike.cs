namespace PixsyAPI.Models
{
    public class PictureLike
    {
        public int PictureLikeID { get; set; }
        public int PictureID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
