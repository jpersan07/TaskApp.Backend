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

    /// <summary>
    /// A method to get all the Tags from the JWT user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var result = await _tagService.GetAllTags(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return Ok(result);
    }

    /// <summary>
    /// A method to get the Tag info by its ID
    /// </summary>
    /// <param name="id">The ID of the tag you want to get the info from</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
      var result = await _tagService.GetTagById(id);
      if (result != null)
        return Ok(result);
      else
        return NotFound();
    }

    /// <summary>
    /// A method to create a tag for the JWT user
    /// </summary>
    /// <param name="newTag">The Tag obj that is going to be created</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTag(CreateTagDto newTag)
    {
      var result = await _tagService.CreateTag(newTag, User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return CreatedAtAction(nameof(GetTagById), new { id = result.Id}, result);
    }

    /// <summary>
    /// A method to update a tag by a new Tag obj and its id
    /// </summary>
    /// <param name="uptTag">The new Tag properties in a new obj</param>
    /// <param name="id">The id of the tag wanting to update</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(UpdateTagDto uptTag, int id)
    {
      await _tagService.UpdateTag(uptTag, id);
      return NoContent();
    }

    /// <summary>
    /// A method to delete a tag by its id
    /// </summary>
    /// <param name="id">The id of the tag</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
      await _tagService.DeleteTag(id);
      return NoContent();
    }

    /// <summary>
    /// A method to delete all tags belonging to the JWT user
    /// </summary>
    /// <returns></returns>
    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDelteUserId()
    {
      await _tagService.BulkDelete(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return NoContent();
    }
  }
}
