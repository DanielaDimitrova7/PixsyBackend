using PixsyAPI.Models;

namespace PixsyAPI.DTOs;

public static class BoardDTO
{
    public sealed class BoardCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public sealed class BoardUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Visibility BoardVisibility { get; set; }
    }

    public sealed class BoardReadDto
    {
        public int BoardID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Visibility BoardVisibility { get; set; }
        public List<int> PictureIds { get; set; } = new();
        public int UserID { get; set; }
    }
}
