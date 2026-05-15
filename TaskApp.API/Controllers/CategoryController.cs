using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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
    public async Task<IActionResult> GetAll(string userId)
    {
      var result = await _categoryService.GetAllCat(userId);
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
      var result = await _categoryService.CreateCategory(newCat, "hola123");
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

    [HttpDelete("bulk/{userId}")]
    public async Task<IActionResult> BulkDelteUserId(string userId)
    {
      await _categoryService.BulkDelete(userId);
      return NoContent();
    }
  }
}
