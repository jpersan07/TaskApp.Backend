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
  public class CategoryController : ControllerBase
  {
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var result = await _categoryService.GetAllCat(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
      var result = await _categoryService.GetCatById(id);
      if (result != null)
        return Ok(result);
      else
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto newCat)
    {
      var result = await _categoryService.CreateCategory(newCat, User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id}, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto uptCat, int id)
    {
      await _categoryService.UpdateCat(uptCat, id);
      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
      await _categoryService.DeleteCat(id);
      return NoContent();
    }

    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDelteUserId()
    {
      await _categoryService.BulkDelete(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return NoContent();
    }
  }
}
