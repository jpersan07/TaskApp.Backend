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
    var allTags = await _repository.GetAllByUserId(userId);
    foreach (var item in allTags)
      await _repository.Delete(item.Id);
  }

  public async Task<TagDto> CreateTag(CreateTagDto newTag, string userId)
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

  public async Task DeleteTag(int id)
  {
    await _repository.Delete(id);
  }

  public async Task<IEnumerable<TagDto>> GetAllTags(string userId)
  {
    var tags = await _repository.GetAllByUserId(userId);
    return tags.Select(tag => new TagDto
    {
      Id = tag.Id,
      Name = tag.Name,
      UserId = userId
    });
  }

  public async Task<TagDto?> GetTagById(int id)
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

  public async Task UpdateTag(UpdateTagDto uptTag, int id)
  {
    var tag = await _repository.GetById(id);

    if (tag != null)
    {
      if (uptTag.Name != null)
        tag.Name = uptTag.Name;
      await _repository.Update(tag);
    }
  }
}
