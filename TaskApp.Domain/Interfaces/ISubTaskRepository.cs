using TaskApp.Domain.Entities;

namespace TaskApp.Domain.Interfaces;

public interface ISubTaskRepository : IRepository<SubTask>
{
  public Task<IEnumerable<SubTask>> GetAllByTaskId(int taskId);
}
