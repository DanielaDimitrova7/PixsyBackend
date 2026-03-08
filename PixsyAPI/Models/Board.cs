namespace PixsyAPI.Models
{
    public class Board
    {
        public int BoardID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserID { get; set; }
        public List<int> PictureIds { get; set; } = new();
        public Visibility BoardVisibility { get; set; } = Visibility.Public;
    }

    public enum Visibility
    {
        Public,
        Private
    }
}
