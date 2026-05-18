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
  /// <summary>
  /// A method to bulk delete all categories from a user
  /// </summary>
  /// <param name="userId">The user that is going to get all the categories deleted</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to create a new category from a Category obj
  /// </summary>
  /// <param name="newCat">The Category obj that is going to be created</param>
  /// <param name="userId">The owner of the category</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<CategoryDto> CreateCategory(CreateCategoryDto newCat, string userId)
  {
    try
    {
      Category _newCat = new Category
      {
        Name = newCat.Name,
        UserId = userId
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

  /// <summary>
  /// A method to delete a category by its id
  /// </summary>
  /// <param name="id">The id of the category that is going to be deleted</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to get all categories from a user by its userId
  /// </summary>
  /// <param name="userId">The owner of all the categories that are going to be retrieved</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to get a category by its id
  /// </summary>
  /// <param name="id">The id of the category you want to retrieve</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to update a category by a new Category obj and its id
  /// </summary>
  /// <param name="uptCat">The new Category properties in a new obj</param>
  /// <param name="id">The id of the category wanting to update</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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
