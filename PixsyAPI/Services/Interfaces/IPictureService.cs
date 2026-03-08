using PixsyAPI.DTOs;

namespace PixsyAPI.Services.Interfaces;

public interface IPictureService
{
    Task<PictureDTO.PictureReadDto> CreateAsync(int requesterUserId, PictureDTO.UploadPictureDto dto, CancellationToken ct);
    Task<List<PictureDTO.PictureReadDto>> GetMineAsync(int requesterUserId, CancellationToken ct);
    Task<List<PictureDTO.PictureReadDto>> GetFeedAsync(int? requesterUserId, CancellationToken ct);
    Task<PictureDTO.PictureReadDto> GetByIdAsync(int? requesterUserId, int pictureId, CancellationToken ct);
    Task<List<CommentDTO.CommentReadDto>> GetCommentsAsync(int? requesterUserId, int pictureId, CancellationToken ct);
    Task<CommentDTO.CommentReadDto> AddCommentAsync(int requesterUserId, int pictureId, CommentDTO.CreateCommentDto dto, CancellationToken ct);
    Task DeleteCommentAsync(int requesterUserId, int pictureId, int commentId, CancellationToken ct);
    Task LikeAsync(int requesterUserId, int pictureId, CancellationToken ct);
    Task UnlikeAsync(int requesterUserId, int pictureId, CancellationToken ct);
    Task DeleteAsync(int requesterUserId, int pictureId, CancellationToken ct);
}
