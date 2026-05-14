using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TagController : ControllerBase
  {
    private readonly ITagService _tagService;
    public TagController(ITagService tagService) => _tagService = tagService;

    [HttpGet]
    public async Task<IActionResult> GetAll(string userId)
    {
      var result = await _tagService.GetAllTags(userId);
      return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
      var result = await _tagService.GetTagById(id);
      if (result != null)
        return Ok(result);
      else
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag(CreateTagDto newTag)
    {
      var result = await _tagService.CreateTag(newTag, "hola123");
      return CreatedAtAction(nameof(GetTagById), new { id = result.Id}, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(UpdateTagDto uptTag, int id)
    {
      await _tagService.UpdateTag(uptTag, id);
      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
      await _tagService.DeleteTag(id);
      return NoContent();
    }

    [HttpDelete("bulk/{userId}")]
    public async Task<IActionResult> BulkDelteUserId(string userId)
    {
      await _tagService.BulkDelete(userId);
      return NoContent();
    }
  }
}
