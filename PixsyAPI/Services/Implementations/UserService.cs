using Microsoft.EntityFrameworkCore;
using PixsyAPI.Data;
using PixsyAPI.DTOs;
using PixsyAPI.ErrorHandling;
using PixsyAPI.Services.Interfaces;

namespace PixsyAPI.Services.Implementations;

public sealed class UserService : IUserService
{
    private readonly PixsyDbContext _db;

    public UserService(PixsyDbContext db)
    {
        _db = db;
    }

    public async Task<UserDTO.UserReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == id, ct);
        if (user == null)
            throw new NotFoundException("Потребителят не е намерен.");
        return Mappers.ToUserReadDto(user);
    }

    public async Task<UserDTO.UserReadDto> UpdateMeAsync(int requesterUserId, UserDTO.UpdateMeRequest dto, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == requesterUserId, ct);
        if (user == null)
            throw new NotFoundException("Потребителят не е намерен.");

        if (dto.DisplayName != null)
        {
            var displayName = dto.DisplayName.Trim();
            if (string.IsNullOrWhiteSpace(displayName))
                throw new BadRequestException("Display name е задължително.");
            user.DisplayName = displayName;
        }

        if (dto.Email != null)
        {
            var email = dto.Email.Trim();
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email е задължителен.");
            var exists = await _db.Users.AnyAsync(u => u.Email == email && u.UserID != requesterUserId, ct);
            if (exists)
                throw new ConflictException("Имейлът вече съществува.");
            user.Email = email;
        }

        await _db.SaveChangesAsync(ct);
        return Mappers.ToUserReadDto(user);
    }

    public async Task DeleteAsync(int requesterUserId, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserID == requesterUserId, ct);
        if (user == null)
            throw new NotFoundException("Потребителят не е намерен.");

        var pictureIds = await _db.Pictures.Where(p => p.UserID == requesterUserId).Select(p => p.PictureID).ToListAsync(ct);
        var pictures = await _db.Pictures.Where(p => p.UserID == requesterUserId).ToListAsync(ct);
        var boards = await _db.Boards.Where(b => b.UserID == requesterUserId).ToListAsync(ct);
        var likesByUser = await _db.PictureLikes.Where(l => l.UserID == requesterUserId).ToListAsync(ct);
        var likesOnUserPictures = await _db.PictureLikes.Where(l => pictureIds.Contains(l.PictureID)).ToListAsync(ct);
        var commentsByUser = await _db.PictureComments.Where(c => c.UserID == requesterUserId).ToListAsync(ct);
        var commentsOnUserPictures = await _db.PictureComments.Where(c => pictureIds.Contains(c.PictureID)).ToListAsync(ct);

        foreach (var otherBoard in await _db.Boards.Where(b => b.UserID != requesterUserId).ToListAsync(ct))
            otherBoard.PictureIds = otherBoard.PictureIds.Where(id => !pictureIds.Contains(id)).ToList();

        foreach (var tag in await _db.Tags.ToListAsync(ct))
            tag.PicturesIds = tag.PicturesIds.Where(id => !pictureIds.Contains(id)).ToList();

        _db.PictureLikes.RemoveRange(likesByUser);
        _db.PictureLikes.RemoveRange(likesOnUserPictures.Where(l => likesByUser.All(x => x.PictureLikeID != l.PictureLikeID)));
        _db.PictureComments.RemoveRange(commentsByUser);
        _db.PictureComments.RemoveRange(commentsOnUserPictures.Where(c => commentsByUser.All(x => x.PictureCommentID != c.PictureCommentID)));
        _db.Pictures.RemoveRange(pictures);
        _db.Boards.RemoveRange(boards);
        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
    }
}
