namespace PixsyAPI.Models
{
    public class Tag
    {
        public int TagID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<int> PicturesIds { get; set; } = new();
    }
}
