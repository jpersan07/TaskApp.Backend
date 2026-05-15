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
    try
    {
      var allTasks = await _repository.GetAllByUserId(userId);
      foreach (var item in allTasks)
        await _repository.Delete(item.Id);
    }
    catch (Exception ex)
    {
      throw new Exception($"Error deleting all categories for user '{userId}'", ex);
    }
  }

  public async Task<CategoryDto> CreateCategory(CreateCategoryDto newCat, string userId)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception($"Error creating the new category '{newCat.Name}'", ex);
    }
  }

  public async Task DeleteCat(int id)
  {
    try
    {
      await _repository.Delete(id);
    }
    catch (Exception ex)
    {
      throw new Exception($"Error deleting the category with id '{id}'", ex);
    }
  }

  public async Task<IEnumerable<CategoryDto>> GetAllCat(string userId)
  {
    try
    {
      var cats = await _repository.GetAllByUserId(userId);
      return cats.Select(cat => new CategoryDto
      {
        Id = cat.Id,
        Name = cat.Name,
        UserId = userId
      });
    }
    catch (Exception ex)
    {
      throw new Exception($"Error retrieving all categories for user '{userId}'", ex);
    }
  }

  public async Task<CategoryDto?> GetCatById(int id)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception($"Error retrieving the category with id '{id}'", ex);
    }
  }

  public async Task UpdateCat(UpdateCategoryDto uptCat, int id)
  {
    try
    {
      var cat = await _repository.GetById(id);

      if (cat != null)
      {
        if (uptCat.Name != null)
          cat.Name = uptCat.Name;
        await _repository.Update(cat);
      }
    }
    catch (Exception ex)
    {
      throw new Exception($"Error updating the category with id '{id}'", ex);
    }
  }
}
