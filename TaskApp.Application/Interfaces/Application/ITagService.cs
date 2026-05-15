using TaskApp.Application.DTOs;

namespace TaskApp.Application.Services;

public interface ITagService
{
  public Task<IEnumerable<TagDto>> GetAllTags(string userId);
  public Task<TagDto?> GetTagById(int id);
  public Task<TagDto> CreateTag(CreateTagDto newTag, string userId);
  public Task UpdateTag(UpdateTagDto uptTag, int id);
  public Task DeleteTag(int id);
  public Task BulkDelete(string userId);
}
