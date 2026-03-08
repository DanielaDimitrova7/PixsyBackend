using Microsoft.EntityFrameworkCore;
using PixsyAPI.Data;
using PixsyAPI.DTOs;
using PixsyAPI.ErrorHandling;
using PixsyAPI.Services.Interfaces;

namespace PixsyAPI.Services.Implementations;

public sealed class BoardService : IBoardService
{
    private readonly PixsyDbContext _db;

    public BoardService(PixsyDbContext db)
    {
        _db = db;
    }

    public async Task<BoardDTO.BoardReadDto> CreateAsync(int requesterUserId, BoardDTO.BoardCreateDto dto, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == requesterUserId, ct);
        if (user == null) throw new NotFoundException("Потребителят не е намерен.");

        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new BadRequestException("Името на board е задължително.");

        var board = new Models.Board
        {
            Name = name,
            Description = dto.Description?.Trim() ?? string.Empty,
            UserID = requesterUserId
        };

        _db.Boards.Add(board);
        await _db.SaveChangesAsync(ct);
        user.BoardsIds.Add(board.BoardID);
        await _db.SaveChangesAsync(ct);
        return Mappers.ToBoardReadDto(board);
    }

    public async Task<List<BoardDTO.BoardReadDto>> GetMineAsync(int requesterUserId, CancellationToken ct)
    {
        var boards = await _db.Boards.AsNoTracking().Where(b => b.UserID == requesterUserId).OrderByDescending(b => b.BoardID).ToListAsync(ct);
        return boards.Select(Mappers.ToBoardReadDto).ToList();
    }

    public async Task<BoardDTO.BoardReadDto> GetByIdAsync(int requesterUserId, int boardId, CancellationToken ct)
    {
        var board = await _db.Boards.AsNoTracking().FirstOrDefaultAsync(b => b.BoardID == boardId, ct);
        if (board == null) throw new NotFoundException("Board не е намерен.");
        if (board.BoardVisibility == Models.Visibility.Private && board.UserID != requesterUserId) throw new ForbiddenException("Нямате достъп до този board.");
        return Mappers.ToBoardReadDto(board);
    }

    public async Task<BoardDTO.BoardReadDto> UpdateAsync(int requesterUserId, int boardId, BoardDTO.BoardUpdateDto dto, CancellationToken ct)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.BoardID == boardId, ct);
        if (board == null) throw new NotFoundException("Board не е намерен.");
        if (board.UserID != requesterUserId) throw new ForbiddenException("Можете да редактирате само вашите boards.");

        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new BadRequestException("Името на board е задължително.");

        board.Name = name;
        board.Description = dto.Description?.Trim() ?? string.Empty;
        board.BoardVisibility = dto.BoardVisibility;
        await _db.SaveChangesAsync(ct);
        return Mappers.ToBoardReadDto(board);
    }

    public async Task AddPictureAsync(int requesterUserId, int boardId, int pictureId, CancellationToken ct)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.BoardID == boardId, ct);
        if (board == null) throw new NotFoundException("Board не е намерен.");
        if (board.UserID != requesterUserId) throw new ForbiddenException("Можете да редактирате само вашите boards.");

        var picture = await _db.Pictures.AsNoTracking().FirstOrDefaultAsync(p => p.PictureID == pictureId, ct);
        if (picture == null) throw new NotFoundException("Снимката не е намерена.");

        if (!board.PictureIds.Contains(pictureId))
        {
            board.PictureIds.Add(pictureId);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemovePictureAsync(int requesterUserId, int boardId, int pictureId, CancellationToken ct)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.BoardID == boardId, ct);
        if (board == null) throw new NotFoundException("Board не е намерен.");
        if (board.UserID != requesterUserId) throw new ForbiddenException("Можете да редактирате само вашите boards.");
        board.PictureIds.Remove(pictureId);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<PictureDTO.PictureReadDto>> GetPicturesAsync(int requesterUserId, int boardId, CancellationToken ct)
    {
        var board = await _db.Boards.AsNoTracking().FirstOrDefaultAsync(b => b.BoardID == boardId, ct);
        if (board == null) throw new NotFoundException("Board не е намерен.");
        if (board.BoardVisibility == Models.Visibility.Private && board.UserID != requesterUserId) throw new ForbiddenException("Нямате достъп до този board.");

        var pictures = await _db.Pictures.AsNoTracking().Where(p => board.PictureIds.Contains(p.PictureID)).OrderByDescending(p => p.PictureID).ToListAsync(ct);
        var authorIds = pictures.Select(p => p.UserID).Distinct().ToList();
        var authors = await _db.Users.AsNoTracking().Where(u => authorIds.Contains(u.UserID)).ToDictionaryAsync(u => u.UserID, ct);
        var pictureIds = pictures.Select(p => p.PictureID).ToList();
        var likeCounts = await _db.PictureLikes.AsNoTracking().Where(l => pictureIds.Contains(l.PictureID)).GroupBy(l => l.PictureID).Select(g => new { PictureID = g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.PictureID, x => x.Count, ct);
        var commentCounts = await _db.PictureComments.AsNoTracking().Where(c => pictureIds.Contains(c.PictureID)).GroupBy(c => c.PictureID).Select(g => new { PictureID = g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.PictureID, x => x.Count, ct);
        var likedPictureIds = await _db.PictureLikes.AsNoTracking().Where(l => l.UserID == requesterUserId && pictureIds.Contains(l.PictureID)).Select(l => l.PictureID).ToListAsync(ct);
        var likedSet = likedPictureIds.ToHashSet();
        return pictures.Select(p => Mappers.ToPictureReadDto(p, authors.GetValueOrDefault(p.UserID), likeCounts.GetValueOrDefault(p.PictureID), commentCounts.GetValueOrDefault(p.PictureID), likedSet.Contains(p.PictureID))).ToList();
    }

    public async Task DeleteAsync(int requesterUserId, int boardId, CancellationToken ct)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.BoardID == boardId, ct);
        if (board == null) throw new NotFoundException("Board не е намерен.");
        if (board.UserID != requesterUserId) throw new ForbiddenException("Можете да триете само вашите boards.");
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == requesterUserId, ct);
        user?.BoardsIds.Remove(board.BoardID);
        _db.Boards.Remove(board);
        await _db.SaveChangesAsync(ct);
    }
}
