using TaskApp.Application.DTOs;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Application.Services;

public class TagService : ITagService
{
  private readonly ITagRepository _repository;
  public TagService(ITagRepository repository)
  {
    _repository = repository;
  }

  /// <summary>
  /// A method to bulk delete all tags from a user
  /// </summary>
  /// <param name="userId">The user that is going to get all the tags deleted</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task BulkDelete(string userId)
  {
    try
    {
      var allTags = await _repository.GetAllByUserId(userId);
      foreach (var item in allTags)
        await _repository.Delete(item.Id);
    }
    catch (Exception ex)
    {
      throw new Exception($"Error deleting all tags for user '{userId}'", ex);
    }
  }

  /// <summary>
  /// A method to create a new tag from a Tag obj
  /// </summary>
  /// <param name="newTag">The Tag obj that is going to be created</param>
  /// <param name="userId">The owner of the tag</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<TagDto> CreateTag(CreateTagDto newTag, string userId)
  {
    try
    {
      Tag _newTag = new Tag
      {
        Name = newTag.Name,
        UserId= userId
      };

      await _repository.Add(_newTag);

      return new TagDto
      {
        Id = _newTag.Id,
        Name = _newTag.Name,
        UserId = userId
      };
    }
    catch (Exception ex)
    {
      throw new Exception($"Error creating the new tag '{newTag.Name}'", ex);
    }
  }

  /// <summary>
  /// A method to delete a tag by its id
  /// </summary>
  /// <param name="id">The id of the tag that is going to be deleted</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task DeleteTag(int id)
  {
    try
    {
      await _repository.Delete(id);
    }
    catch (Exception ex)
    {
      throw new Exception($"Error deleting the tag with id '{id}'", ex);
    }
  }

  /// <summary>
  /// A method to get all tags from a user by its userId
  /// </summary>
  /// <param name="userId">The owner of all the tags that are going to be retrieved</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<IEnumerable<TagDto>> GetAllTags(string userId)
  {
    try
    {
      var tags = await _repository.GetAllByUserId(userId);
      return tags.Select(tag => new TagDto
      {
        Id = tag.Id,
        Name = tag.Name,
        UserId = userId
      });
    }
    catch (Exception ex)
    {
      throw new Exception($"Error retrieving all tags for user '{userId}'", ex);
    }
  }

  /// <summary>
  /// A method to get a tag by its id
  /// </summary>
  /// <param name="id">The id of the tag you want to retrieve</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<TagDto?> GetTagById(int id)
  {
    try
    {
      var tag = await _repository.GetById(id);

      if (tag != null)
        return new TagDto
        {
          Id = tag.Id,
          Name = tag.Name,
          UserId = tag.UserId
        };
      else
        return null;
    }
    catch (Exception ex)
    {
      throw new Exception($"Error retrieving the tag with id '{id}'", ex);
    }
  }

  /// <summary>
  /// A method to update a tag by a new Tag obj and its id
  /// </summary>
  /// <param name="uptTag">The new Tag properties in a new obj</param>
  /// <param name="id">The id of the tag wanting to update</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task UpdateTag(UpdateTagDto uptTag, int id)
  {
    try
    {
      var tag = await _repository.GetById(id);

      if (tag != null)
      {
        if (uptTag.Name != null)
          tag.Name = uptTag.Name;
        await _repository.Update(tag);
      }
    }
    catch (Exception ex)
    {
      throw new Exception($"Error updating the tag with id '{id}'", ex);
    }
  }
}
