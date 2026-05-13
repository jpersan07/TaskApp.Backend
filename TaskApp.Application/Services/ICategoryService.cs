using TaskApp.Application.DTOs;

namespace TaskApp.Application.Services;

public interface ICategoryService
{
  public Task<IEnumerable<CategoryDto>> GetAllCat(string userId);
  public Task <CategoryDto?> GetCatById(int id);
  public Task<CategoryDto> CreateCategory(CreateCategoryDto newCat, string userId);
  public Task UpdateCat(UpdateCategoryDto uptCat, int id);
  public Task DeleteCat(int id);
  public Task BulkDelete(string userId);
}
