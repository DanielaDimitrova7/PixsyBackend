using PixsyAPI.DTOs;

namespace PixsyAPI.Services.Interfaces;

public interface IBoardService
{
    Task<BoardDTO.BoardReadDto> CreateAsync(int requesterUserId, BoardDTO.BoardCreateDto dto, CancellationToken ct);
    Task<List<BoardDTO.BoardReadDto>> GetMineAsync(int requesterUserId, CancellationToken ct);
    Task<BoardDTO.BoardReadDto> GetByIdAsync(int requesterUserId, int boardId, CancellationToken ct);
    Task<BoardDTO.BoardReadDto> UpdateAsync(int requesterUserId, int boardId, BoardDTO.BoardUpdateDto dto, CancellationToken ct);
    Task AddPictureAsync(int requesterUserId, int boardId, int pictureId, CancellationToken ct);
    Task RemovePictureAsync(int requesterUserId, int boardId, int pictureId, CancellationToken ct);
    Task<List<PictureDTO.PictureReadDto>> GetPicturesAsync(int requesterUserId, int boardId, CancellationToken ct);
    Task DeleteAsync(int requesterUserId, int boardId, CancellationToken ct);
}
