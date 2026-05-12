using TaskApp.Application.DTOs;

namespace TaskApp.Application.Services;

public interface IAppTaskService
{
  public Task<IEnumerable<AppTaskDto>> GetAllTaskUser(string userId);
  public Task<AppTaskDto?> GetTaskById(int id);
  public Task<AppTaskDto> CreateTask(CreateAppTaskDto newTask, string userId);
  public Task UpdateTask(UpdateAppTaskDto uptTask, int id);
  public Task DeleteTask (int id);
  public Task BulkDelete(string userId);
}
