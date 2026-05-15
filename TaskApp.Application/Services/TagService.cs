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

  public async Task<TagDto> CreateTag(CreateTagDto newTag, string userId)
  {
    try
    {
      Tag _newTag = new Tag
      {
        Name = newTag.Name
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
