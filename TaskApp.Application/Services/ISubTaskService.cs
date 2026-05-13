using TaskApp.Application.DTOs;

namespace TaskApp.Application.Services;

public interface ISubTaskService
{
  public Task<IEnumerable<SubTaskDto>> GetAllSubTasks(int taskId);
  public Task<SubTaskDto> GetSubTaskById(int id);
  public Task<SubTaskDto> CreateSubTask(CreateSubTaskDto newSub);
  public Task UpdateSubTask(UpdateSubTaskDto uptSub, int id);
  public Task DeleteSubTask(int id);
  public Task BulkDelete(int taskId);
}
