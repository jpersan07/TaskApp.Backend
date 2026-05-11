using TaskApp.Domain.Entities;

namespace TaskApp.Domain.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
  public Task<IEnumerable<Tag>> GetAllByUserId(string userId);
}
