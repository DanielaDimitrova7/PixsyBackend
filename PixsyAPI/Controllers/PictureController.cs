using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixsyAPI.DTOs;
using PixsyAPI.Services.Interfaces;
using PixsyAPI.Services.Security;

namespace PixsyAPI.Controllers;

[ApiController]
[Route("api/v1/pictures")]
public sealed class PictureController : ControllerBase
{
    private readonly IPictureService _pictures;
    public PictureController(IPictureService pictures) => _pictures = pictures;

    [HttpGet("feed")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PictureDTO.PictureReadDto>>> GetFeed(CancellationToken ct)
        => Ok(await _pictures.GetFeedAsync(User.GetUserIdOrNull(), ct));

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<List<PictureDTO.PictureReadDto>>> GetMine(CancellationToken ct)
        => Ok(await _pictures.GetMineAsync(User.GetUserIdOrThrow(), ct));

    [HttpPost]
    [Authorize]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<PictureDTO.PictureReadDto>> Create([FromForm] PictureDTO.UploadPictureDto dto, CancellationToken ct)
        => Ok(await _pictures.CreateAsync(User.GetUserIdOrThrow(), dto, ct));

    [HttpGet("{pictureId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PictureDTO.PictureReadDto>> GetById(int pictureId, CancellationToken ct)
        => Ok(await _pictures.GetByIdAsync(User.GetUserIdOrNull(), pictureId, ct));

    [HttpGet("{pictureId:int}/comments")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CommentDTO.CommentReadDto>>> GetComments(int pictureId, CancellationToken ct)
        => Ok(await _pictures.GetCommentsAsync(User.GetUserIdOrNull(), pictureId, ct));

    [HttpPost("{pictureId:int}/comments")]
    [Authorize]
    public async Task<ActionResult<CommentDTO.CommentReadDto>> AddComment(int pictureId, [FromBody] CommentDTO.CreateCommentDto dto, CancellationToken ct)
        => Ok(await _pictures.AddCommentAsync(User.GetUserIdOrThrow(), pictureId, dto, ct));

    [HttpDelete("{pictureId:int}/comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int pictureId, int commentId, CancellationToken ct)
    {
        await _pictures.DeleteCommentAsync(User.GetUserIdOrThrow(), pictureId, commentId, ct);
        return NoContent();
    }

    [HttpPost("{pictureId:int}/likes")]
    [Authorize]
    public async Task<IActionResult> Like(int pictureId, CancellationToken ct)
    {
        await _pictures.LikeAsync(User.GetUserIdOrThrow(), pictureId, ct);
        return NoContent();
    }

    [HttpDelete("{pictureId:int}/likes")]
    [Authorize]
    public async Task<IActionResult> Unlike(int pictureId, CancellationToken ct)
    {
        await _pictures.UnlikeAsync(User.GetUserIdOrThrow(), pictureId, ct);
        return NoContent();
    }

    [HttpDelete("{pictureId:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int pictureId, CancellationToken ct)
    {
        await _pictures.DeleteAsync(User.GetUserIdOrThrow(), pictureId, ct);
        return NoContent();
    }
}
