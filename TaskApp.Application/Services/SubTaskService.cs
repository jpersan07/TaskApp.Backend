using TaskApp.Application.DTOs;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Application.Services;

public class SubTaskService : ISubTaskService
{
  private readonly ISubTaskRepository _repository;
  public SubTaskService(ISubTaskRepository repository)
  {
    _repository = repository;
  }

  public async Task BulkDelete(int taskId)
  {
    var allSubTasks = await _repository.GetAllByTaskId(taskId);
    foreach (var item in allSubTasks)
      await _repository.Delete(item.Id);
  }

  public async Task<SubTaskDto> CreateSubTask(CreateSubTaskDto newSub)
  {
    SubTask _newSub = new SubTask
    {
      Title = newSub.Title,
      TaskId = newSub.TaskId,
      IsCompleted = newSub.IsCompleted
    };

    await _repository.Add(_newSub);

    return new SubTaskDto
    {
      Id = _newSub.Id,
      Title = _newSub.Title,
      TaskId = _newSub.TaskId,
      IsCompleted = _newSub.IsCompleted
    };
  }

  public async Task DeleteSubTask(int id)
  {
    await _repository.Delete(id);
  }

  public async Task<IEnumerable<SubTaskDto>> GetAllSubTasks(int taskId)
  {
    var subTasks = await _repository.GetAllByTaskId(taskId);
    return subTasks.Select(sub => new SubTaskDto
    {
      Id = sub.Id,
      Title = sub.Title,
      TaskId = sub.TaskId,
      IsCompleted = sub.IsCompleted
    });
  }

  public async Task<SubTaskDto> GetSubTaskById(int id)
  {
    var sub = await _repository.GetById(id);

    if (sub != null)
      return new SubTaskDto
      {
        Id = sub.Id,
        Title = sub.Title,
        TaskId = sub.TaskId,
        IsCompleted = sub.IsCompleted
      };
    else
      return null!;
  }

  public async Task UpdateSubTask(UpdateSubTaskDto uptSub, int id)
  {
    var sub = await _repository.GetById(id);

    if (sub != null)
    {
      if (uptSub.Title != null)
        sub.Title = uptSub.Title;
      if (uptSub.IsCompleted != null)
        sub.IsCompleted = uptSub.IsCompleted.Value;
      await _repository.Update(sub);
    }
  }
}
