using TaskApp.Application.DTOs;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Application.Services;

public class CategoryService : ICategoryService
{
  private readonly ICategoryRepository _repository;
  public CategoryService(ICategoryRepository repository)
  {
    _repository = repository;
  }
  public async Task BulkDelete(string userId)
  {
    var allTasks = await _repository.GetAllByUserId(userId);
    foreach (var item in allTasks)
      await _repository.Delete(item.Id);

  }

  public async Task<CategoryDto> CreateCategory(CreateCategoryDto newCat, string userId)
  {
    Category _newCat = new Category
    {
      Name = newCat.Name
    };

    await _repository.Add(_newCat);

    return new CategoryDto
    {
      Id = _newCat.Id,
      Name = _newCat.Name,
      UserId = userId
    };
  }

  public async Task DeleteCat(int id)
  {
    await _repository.Delete(id);
  }

  public async Task<IEnumerable<CategoryDto>> GetAllCat(string userId)
  {
    var cats = await _repository.GetAllByUserId(userId);
    return cats.Select(cat => new CategoryDto
    {
      Id = cat.Id,
      Name = cat.Name,
      UserId = userId
    });
  }

  public async Task<CategoryDto?> GetCatById(int id)
  {
    var cat = await _repository.GetById(id);

    if (cat != null)
      return new CategoryDto
      {
        Id = cat.Id,
        Name = cat.Name,
        UserId = cat.UserId
      };
    else
      return null;
  }

  public async Task UpdateCat(UpdateCategoryDto uptCat, int id)
  {
    var cat = await _repository.GetById(id);

    if (cat != null)
    {
      if (uptCat.Name != null)
        cat.Name = uptCat.Name;
      await _repository.Update(cat);
    }
  }
}
