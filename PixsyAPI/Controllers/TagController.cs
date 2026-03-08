using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixsyAPI.DTOs;
using PixsyAPI.Services.Interfaces;

namespace PixsyAPI.Controllers;

[ApiController]
[Route("api/v1/tags")]
public sealed class TagController : ControllerBase
{
    private readonly ITagService _tags;
    public TagController(ITagService tags) => _tags = tags;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TagDTO.TagReadDto>>> GetAll(CancellationToken ct)
        => Ok(await _tags.GetAllAsync(ct));

    [HttpGet("{tagId:int}")]
    [Authorize]
    public async Task<ActionResult<TagDTO.TagReadDto>> GetById(int tagId, CancellationToken ct)
        => Ok(await _tags.GetByIdAsync(tagId, ct));

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TagDTO.TagReadDto>> Create([FromBody] TagDTO.CreateTagDto dto, CancellationToken ct)
        => Ok(await _tags.CreateAsync(dto, ct));

    [HttpDelete("{tagId:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int tagId, CancellationToken ct)
    {
        await _tags.DeleteAsync(tagId, ct);
        return NoContent();
    }
}
