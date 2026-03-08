using PixsyAPI.DTOs;

namespace PixsyAPI.Services.Interfaces;

public interface ITagService
{
    Task<TagDTO.TagReadDto> CreateAsync(TagDTO.CreateTagDto dto, CancellationToken ct);
    Task<List<TagDTO.TagReadDto>> GetAllAsync(CancellationToken ct);
    Task<TagDTO.TagReadDto> GetByIdAsync(int tagId, CancellationToken ct);
    Task DeleteAsync(int tagId, CancellationToken ct);
}
