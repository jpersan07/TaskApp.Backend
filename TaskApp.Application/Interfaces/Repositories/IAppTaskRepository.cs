using TaskApp.Domain.Entities;

namespace TaskApp.Domain.Interfaces;

public interface IAppTaskRepository : IRepository<AppTask>
{
  public Task<IEnumerable<AppTask>> GetAllByUserId(string userId);
}
