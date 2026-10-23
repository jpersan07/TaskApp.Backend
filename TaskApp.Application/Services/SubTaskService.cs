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

  /// <summary>
  /// A method to bulk delete all subtasks from a parent task
  /// </summary>
  /// <param name="taskId">The id of the parent task whose subtasks are going to be deleted</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task BulkDelete(int taskId)
  {
    var allSubTasks = await _repository.GetAllByTaskId(taskId);
    foreach (var item in allSubTasks)
      await _repository.Delete(item.Id);
  }

  /// <summary>
  /// A method to create a new subtask from a SubTask obj
  /// </summary>
  /// <param name="newSub">The SubTask obj that is going to be created</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to delete a subtask by its id
  /// </summary>
  /// <param name="id">The id of the subtask that is going to be deleted</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task DeleteSubTask(int id)
  {
    await _repository.Delete(id);
  }

  /// <summary>
  /// A method to get all subtasks belonging to a parent task
  /// </summary>
  /// <param name="taskId">The id of the parent task</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to get a subtask by its id
  /// </summary>
  /// <param name="id">The id of the subtask you want to retrieve</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to update a subtask by a new SubTask obj and its id
  /// </summary>
  /// <param name="uptSub">The new SubTask properties in a new obj</param>
  /// <param name="id">The id of the subtask wanting to update</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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
