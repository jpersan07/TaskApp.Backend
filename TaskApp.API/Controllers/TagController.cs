using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class TagController : ControllerBase
  {
    private readonly ITagService _tagService;
    public TagController(ITagService tagService) => _tagService = tagService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var result = await _tagService.GetAllTags(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
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
      var result = await _tagService.CreateTag(newTag, User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
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

    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDelteUserId()
    {
      await _tagService.BulkDelete(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return NoContent();
    }
  }
}
