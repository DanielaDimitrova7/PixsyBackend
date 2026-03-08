namespace PixsyAPI.DTOs;

public static class TagDTO
{
    public sealed class CreateTagDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public sealed class TagReadDto
    {
        public int TagID { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
