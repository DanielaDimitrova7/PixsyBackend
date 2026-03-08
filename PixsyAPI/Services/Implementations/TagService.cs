using Microsoft.EntityFrameworkCore;
using PixsyAPI.Data;
using PixsyAPI.DTOs;
using PixsyAPI.ErrorHandling;
using PixsyAPI.Services.Interfaces;

namespace PixsyAPI.Services.Implementations;

public sealed class TagService : ITagService
{
    private readonly PixsyDbContext _db;

    public TagService(PixsyDbContext db)
    {
        _db = db;
    }

    public async Task<TagDTO.TagReadDto> CreateAsync(TagDTO.CreateTagDto dto, CancellationToken ct)
    {
        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new BadRequestException("Името на тага е задължително.");
        var existing = await _db.Tags.FirstOrDefaultAsync(t => t.Name == name, ct);
        if (existing != null) return Mappers.ToTagReadDto(existing);

        var tag = new Models.Tag { Name = name };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync(ct);
        return Mappers.ToTagReadDto(tag);
    }

    public async Task<List<TagDTO.TagReadDto>> GetAllAsync(CancellationToken ct)
    {
        var tags = await _db.Tags.AsNoTracking().OrderBy(t => t.Name).ToListAsync(ct);
        return tags.Select(Mappers.ToTagReadDto).ToList();
    }

    public async Task<TagDTO.TagReadDto> GetByIdAsync(int tagId, CancellationToken ct)
    {
        var tag = await _db.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.TagID == tagId, ct);
        if (tag == null) throw new NotFoundException("Тагът не е намерен.");
        return Mappers.ToTagReadDto(tag);
    }

    public async Task DeleteAsync(int tagId, CancellationToken ct)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.TagID == tagId, ct);
        if (tag == null) throw new NotFoundException("Тагът не е намерен.");
        foreach (var picture in await _db.Pictures.Where(p => p.TagsIds.Contains(tagId)).ToListAsync(ct))
            picture.TagsIds.Remove(tagId);
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(ct);
    }
}
