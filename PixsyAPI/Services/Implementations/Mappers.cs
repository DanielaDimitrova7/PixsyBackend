using PixsyAPI.DTOs;
using PixsyAPI.Models;

namespace PixsyAPI.Services.Implementations;

internal static class Mappers
{
    public static UserDTO.UserReadDto ToUserReadDto(User user) => new()
    {
        UserID = user.UserID,
        UserName = user.UserName,
        DisplayName = user.DisplayName,
        Email = user.Email,
        Role = user.Role
    };

    public static BoardDTO.BoardReadDto ToBoardReadDto(Board board) => new()
    {
        BoardID = board.BoardID,
        Name = board.Name,
        Description = board.Description,
        BoardVisibility = board.BoardVisibility,
        PictureIds = board.PictureIds.ToList(),
        UserID = board.UserID
    };

    public static PictureDTO.PictureReadDto ToPictureReadDto(
        Picture picture,
        User? author = null,
        int likeCount = 0,
        int commentCount = 0,
        bool isLikedByCurrentUser = false) => new()
    {
        PictureID = picture.PictureID,
        UserID = picture.UserID,
        UserName = author?.UserName ?? string.Empty,
        UserDisplayName = author?.DisplayName ?? string.Empty,
        ImageUrl = picture.ImagePath,
        OriginalFileName = picture.OriginalFileName,
        ContentType = picture.ContentType,
        CreatedAtUtc = picture.CreatedAtUtc,
        TagsIds = picture.TagsIds.ToList(),
        LikeCount = likeCount,
        CommentCount = commentCount,
        IsLikedByCurrentUser = isLikedByCurrentUser
    };

    public static CommentDTO.CommentReadDto ToCommentReadDto(PictureComment comment, User? author = null, int? requesterUserId = null) => new()
    {
        PictureCommentID = comment.PictureCommentID,
        PictureID = comment.PictureID,
        UserID = comment.UserID,
        UserName = author?.UserName ?? string.Empty,
        UserDisplayName = author?.DisplayName ?? string.Empty,
        Content = comment.Content,
        CreatedAtUtc = comment.CreatedAtUtc,
        IsOwner = requesterUserId.HasValue && requesterUserId.Value == comment.UserID
    };

    public static TagDTO.TagReadDto ToTagReadDto(Tag tag) => new()
    {
        TagID = tag.TagID,
        Name = tag.Name
    };
}
