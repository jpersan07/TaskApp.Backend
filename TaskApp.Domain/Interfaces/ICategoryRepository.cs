using TaskApp.Domain.Entities;

namespace TaskApp.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
  public Task<IEnumerable<Category>> GetAllByUserId(string userId);
}
