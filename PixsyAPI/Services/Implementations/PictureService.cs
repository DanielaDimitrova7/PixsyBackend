using Microsoft.EntityFrameworkCore;
using PixsyAPI.Data;
using PixsyAPI.DTOs;
using PixsyAPI.ErrorHandling;
using PixsyAPI.Services.Interfaces;

namespace PixsyAPI.Services.Implementations;

public sealed class PictureService : IPictureService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif",
        "image/bmp"
    };

    private readonly PixsyDbContext _db;
    private readonly IWebHostEnvironment _environment;

    public PictureService(PixsyDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public async Task<PictureDTO.PictureReadDto> CreateAsync(int requesterUserId, PictureDTO.UploadPictureDto dto, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == requesterUserId, ct);
        if (user == null) throw new NotFoundException("Потребителят не е намерен.");

        if (dto.File == null || dto.File.Length == 0)
            throw new BadRequestException("Файлът е задължителен.");

        if (!AllowedContentTypes.Contains(dto.File.ContentType))
            throw new BadRequestException("Позволени са само изображения.");

        var tagIds = (dto.Tags ?? new List<int>()).Distinct().ToList();
        var existingTagIds = await _db.Tags.Where(t => tagIds.Contains(t.TagID)).Select(t => t.TagID).ToListAsync(ct);
        var missingTagIds = tagIds.Except(existingTagIds).ToList();
        if (missingTagIds.Count > 0) throw new BadRequestException($"Несъществуващи тагове: {string.Join(", ", missingTagIds)}");

        var uploadsRoot = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "pictures");
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(dto.File.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = dto.File.ContentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                _ => ".jpg"
            };

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsRoot, storedFileName);

        await using (var stream = File.Create(filePath))
        {
            await dto.File.CopyToAsync(stream, ct);
        }

        var picture = new Models.Picture
        {
            UserID = requesterUserId,
            TagsIds = existingTagIds,
            ImagePath = $"/uploads/pictures/{storedFileName}",
            OriginalFileName = Path.GetFileName(dto.File.FileName),
            ContentType = dto.File.ContentType,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Pictures.Add(picture);
        await _db.SaveChangesAsync(ct);

        if (!user.UploadsIds.Contains(picture.PictureID))
            user.UploadsIds.Add(picture.PictureID);

        var tags = await _db.Tags.Where(t => existingTagIds.Contains(t.TagID)).ToListAsync(ct);
        foreach (var tag in tags)
        {
            if (!tag.PicturesIds.Contains(picture.PictureID))
                tag.PicturesIds.Add(picture.PictureID);
        }

        await _db.SaveChangesAsync(ct);
        return Mappers.ToPictureReadDto(picture, user);
    }

    public async Task<List<PictureDTO.PictureReadDto>> GetMineAsync(int requesterUserId, CancellationToken ct)
    {
        return await BuildPictureDtosAsync(
            await _db.Pictures
                .AsNoTracking()
                .Where(p => p.UserID == requesterUserId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ThenByDescending(p => p.PictureID)
                .ToListAsync(ct),
            requesterUserId,
            ct);
    }

    public async Task<List<PictureDTO.PictureReadDto>> GetFeedAsync(int? requesterUserId, CancellationToken ct)
    {
        var pictures = await _db.Pictures
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAtUtc)
            .ThenByDescending(p => p.PictureID)
            .ToListAsync(ct);

        return await BuildPictureDtosAsync(pictures, requesterUserId, ct);
    }

    public async Task<PictureDTO.PictureReadDto> GetByIdAsync(int? requesterUserId, int pictureId, CancellationToken ct)
    {
        var picture = await _db.Pictures.AsNoTracking().FirstOrDefaultAsync(p => p.PictureID == pictureId, ct);
        if (picture == null) throw new NotFoundException("Снимката не е намерена.");

        var author = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == picture.UserID, ct);
        var likeCount = await _db.PictureLikes.AsNoTracking().CountAsync(l => l.PictureID == pictureId, ct);
        var commentCount = await _db.PictureComments.AsNoTracking().CountAsync(c => c.PictureID == pictureId, ct);
        var isLiked = requesterUserId.HasValue && await _db.PictureLikes.AsNoTracking().AnyAsync(l => l.PictureID == pictureId && l.UserID == requesterUserId.Value, ct);

        return Mappers.ToPictureReadDto(picture, author, likeCount, commentCount, isLiked);
    }

    public async Task<List<CommentDTO.CommentReadDto>> GetCommentsAsync(int? requesterUserId, int pictureId, CancellationToken ct)
    {
        var pictureExists = await _db.Pictures.AsNoTracking().AnyAsync(p => p.PictureID == pictureId, ct);
        if (!pictureExists) throw new NotFoundException("Снимката не е намерена.");

        var comments = await _db.PictureComments
            .AsNoTracking()
            .Where(c => c.PictureID == pictureId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ThenByDescending(c => c.PictureCommentID)
            .ToListAsync(ct);

        var authorIds = comments.Select(c => c.UserID).Distinct().ToList();
        var authors = await _db.Users.AsNoTracking().Where(u => authorIds.Contains(u.UserID)).ToDictionaryAsync(u => u.UserID, ct);

        return comments.Select(c => Mappers.ToCommentReadDto(c, authors.GetValueOrDefault(c.UserID), requesterUserId)).ToList();
    }

    public async Task<CommentDTO.CommentReadDto> AddCommentAsync(int requesterUserId, int pictureId, CommentDTO.CreateCommentDto dto, CancellationToken ct)
    {
        var content = dto.Content?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(content)) throw new BadRequestException("Коментарът не може да е празен.");
        if (content.Length > 1000) throw new BadRequestException("Коментарът е твърде дълъг.");

        var pictureExists = await _db.Pictures.AsNoTracking().AnyAsync(p => p.PictureID == pictureId, ct);
        if (!pictureExists) throw new NotFoundException("Снимката не е намерена.");

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == requesterUserId, ct);
        if (user == null) throw new NotFoundException("Потребителят не е намерен.");

        var comment = new Models.PictureComment
        {
            PictureID = pictureId,
            UserID = requesterUserId,
            Content = content,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.PictureComments.Add(comment);
        await _db.SaveChangesAsync(ct);
        return Mappers.ToCommentReadDto(comment, user, requesterUserId);
    }

    public async Task DeleteCommentAsync(int requesterUserId, int pictureId, int commentId, CancellationToken ct)
    {
        var comment = await _db.PictureComments.FirstOrDefaultAsync(c => c.PictureCommentID == commentId && c.PictureID == pictureId, ct);
        if (comment == null) throw new NotFoundException("Коментарът не е намерен.");
        if (comment.UserID != requesterUserId) throw new ForbiddenException("Можете да триете само вашите коментари.");
        _db.PictureComments.Remove(comment);
        await _db.SaveChangesAsync(ct);
    }

    public async Task LikeAsync(int requesterUserId, int pictureId, CancellationToken ct)
    {
        var pictureExists = await _db.Pictures.AsNoTracking().AnyAsync(p => p.PictureID == pictureId, ct);
        if (!pictureExists) throw new NotFoundException("Снимката не е намерена.");

        var existing = await _db.PictureLikes.FirstOrDefaultAsync(l => l.PictureID == pictureId && l.UserID == requesterUserId, ct);
        if (existing != null) return;

        _db.PictureLikes.Add(new Models.PictureLike
        {
            PictureID = pictureId,
            UserID = requesterUserId,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);
    }

    public async Task UnlikeAsync(int requesterUserId, int pictureId, CancellationToken ct)
    {
        var existing = await _db.PictureLikes.FirstOrDefaultAsync(l => l.PictureID == pictureId && l.UserID == requesterUserId, ct);
        if (existing == null) return;
        _db.PictureLikes.Remove(existing);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int requesterUserId, int pictureId, CancellationToken ct)
    {
        var picture = await _db.Pictures.FirstOrDefaultAsync(p => p.PictureID == pictureId, ct);
        if (picture == null) throw new NotFoundException("Снимката не е намерена.");
        if (picture.UserID != requesterUserId) throw new ForbiddenException("Можете да триете само вашите снимки.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == requesterUserId, ct);
        user?.UploadsIds.Remove(picture.PictureID);

        foreach (var board in await _db.Boards.Where(b => b.PictureIds.Contains(picture.PictureID)).ToListAsync(ct))
            board.PictureIds.Remove(picture.PictureID);

        foreach (var tag in await _db.Tags.Where(t => picture.TagsIds.Contains(t.TagID) || t.PicturesIds.Contains(picture.PictureID)).ToListAsync(ct))
            tag.PicturesIds.Remove(picture.PictureID);

        var likes = await _db.PictureLikes.Where(l => l.PictureID == picture.PictureID).ToListAsync(ct);
        var comments = await _db.PictureComments.Where(c => c.PictureID == picture.PictureID).ToListAsync(ct);
        _db.PictureLikes.RemoveRange(likes);
        _db.PictureComments.RemoveRange(comments);

        var relativeImagePath = picture.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), relativeImagePath);
        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        _db.Pictures.Remove(picture);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<List<PictureDTO.PictureReadDto>> BuildPictureDtosAsync(List<Models.Picture> pictures, int? requesterUserId, CancellationToken ct)
    {
        if (pictures.Count == 0) return new List<PictureDTO.PictureReadDto>();

        var authorIds = pictures.Select(p => p.UserID).Distinct().ToList();
        var authors = await _db.Users.AsNoTracking()
            .Where(u => authorIds.Contains(u.UserID))
            .ToDictionaryAsync(u => u.UserID, ct);

        var pictureIds = pictures.Select(p => p.PictureID).ToList();

        var likeCounts = await _db.PictureLikes.AsNoTracking()
            .Where(l => pictureIds.Contains(l.PictureID))
            .GroupBy(l => l.PictureID)
            .Select(g => new { PictureID = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PictureID, x => x.Count, ct);

        var commentCounts = await _db.PictureComments.AsNoTracking()
            .Where(c => pictureIds.Contains(c.PictureID))
            .GroupBy(c => c.PictureID)
            .Select(g => new { PictureID = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PictureID, x => x.Count, ct);

        HashSet<int> likedPictureIds = new();
        if (requesterUserId.HasValue)
        {
            likedPictureIds = (await _db.PictureLikes.AsNoTracking()
                .Where(l => l.UserID == requesterUserId.Value && pictureIds.Contains(l.PictureID))
                .Select(l => l.PictureID)
                .ToListAsync(ct)).ToHashSet();
        }

        return pictures
            .Select(p => Mappers.ToPictureReadDto(
                p,
                authors.GetValueOrDefault(p.UserID),
                likeCounts.GetValueOrDefault(p.PictureID),
                commentCounts.GetValueOrDefault(p.PictureID),
                likedPictureIds.Contains(p.PictureID)))
            .ToList();
    }
}
