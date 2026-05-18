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

    /// <summary>
    /// A method to get all the Categories from the JWT user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var result = await _categoryService.GetAllCat(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return Ok(result);
    }

    /// <summary>
    /// A method to get the Category info by its ID
    /// </summary>
    /// <param name="id">The ID of the category you want to get the info from</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
      var result = await _categoryService.GetCatById(id);
      if (result != null)
        return Ok(result);
      else
        return NotFound();
    }

    /// <summary>
    /// A method to create a category for the JWT user
    /// </summary>
    /// <param name="newCat">The Category obj that is going to be created</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto newCat)
    {
      var result = await _categoryService.CreateCategory(newCat, User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id}, result);
    }

    /// <summary>
    /// A method to update a category by a new Category obj and its id
    /// </summary>
    /// <param name="uptCat">The new Category properties in a new obj</param>
    /// <param name="id">The id of the category wanting to update</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto uptCat, int id)
    {
      await _categoryService.UpdateCat(uptCat, id);
      return NoContent();
    }

    /// <summary>
    /// A method to delete a category by its id
    /// </summary>
    /// <param name="id">The id of the category</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
      await _categoryService.DeleteCat(id);
      return NoContent();
    }

    /// <summary>
    /// A method to delete all categories belonging to the JWT user
    /// </summary>
    /// <returns></returns>
    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDelteUserId()
    {
      await _categoryService.BulkDelete(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
      return NoContent();
    }
  }
}
