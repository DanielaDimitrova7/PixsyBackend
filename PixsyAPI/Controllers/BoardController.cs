using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixsyAPI.DTOs;
using PixsyAPI.Services.Interfaces;
using PixsyAPI.Services.Security;

namespace PixsyAPI.Controllers;

[ApiController]
[Route("api/v1/boards")]
public sealed class BoardController : ControllerBase
{
    private readonly IBoardService _boards;
    public BoardController(IBoardService boards) => _boards = boards;

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<List<BoardDTO.BoardReadDto>>> GetMine(CancellationToken ct)
        => Ok(await _boards.GetMineAsync(User.GetUserIdOrThrow(), ct));

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BoardDTO.BoardReadDto>> Create([FromBody] BoardDTO.BoardCreateDto dto, CancellationToken ct)
        => Ok(await _boards.CreateAsync(User.GetUserIdOrThrow(), dto, ct));

    [HttpGet("{boardId:int}")]
    [Authorize]
    public async Task<ActionResult<BoardDTO.BoardReadDto>> GetById(int boardId, CancellationToken ct)
        => Ok(await _boards.GetByIdAsync(User.GetUserIdOrThrow(), boardId, ct));

    [HttpPut("{boardId:int}")]
    [Authorize]
    public async Task<ActionResult<BoardDTO.BoardReadDto>> Update(int boardId, [FromBody] BoardDTO.BoardUpdateDto dto, CancellationToken ct)
        => Ok(await _boards.UpdateAsync(User.GetUserIdOrThrow(), boardId, dto, ct));

    [HttpDelete("{boardId:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int boardId, CancellationToken ct)
    {
        await _boards.DeleteAsync(User.GetUserIdOrThrow(), boardId, ct);
        return NoContent();
    }

    [HttpPost("{boardId:int}/pictures/{pictureId:int}")]
    [Authorize]
    public async Task<IActionResult> AddPicture(int boardId, int pictureId, CancellationToken ct)
    {
        await _boards.AddPictureAsync(User.GetUserIdOrThrow(), boardId, pictureId, ct);
        return NoContent();
    }

    [HttpDelete("{boardId:int}/pictures/{pictureId:int}")]
    [Authorize]
    public async Task<IActionResult> RemovePicture(int boardId, int pictureId, CancellationToken ct)
    {
        await _boards.RemovePictureAsync(User.GetUserIdOrThrow(), boardId, pictureId, ct);
        return NoContent();
    }

    [HttpGet("{boardId:int}/pictures")]
    [Authorize]
    public async Task<ActionResult<List<PictureDTO.PictureReadDto>>> GetPictures(int boardId, CancellationToken ct)
        => Ok(await _boards.GetPicturesAsync(User.GetUserIdOrThrow(), boardId, ct));
}
