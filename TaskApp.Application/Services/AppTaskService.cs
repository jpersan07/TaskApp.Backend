using System;
using TaskApp.Application.DTOs;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Application.Services;

public class AppTaskService : IAppTaskService
{
  private readonly IAppTaskRepository _repository;
  public AppTaskService(IAppTaskRepository repository)
  {
    _repository = repository;
  }

  public async Task BulkDelete(string userId)
  {
    var allTasks = await _repository.GetAllByUserId(userId);
    foreach (var item in allTasks)
      await _repository.Delete(item.Id);
  }

  public async Task<AppTaskDto> CreateTask(CreateAppTaskDto newTask, string userId)
  {
    AppTask _newTask = new AppTask
    {
      Title = newTask.Title,
      Description = newTask.Description,
      DueDate = newTask.DueDate,
      UserId = userId
    };

    await _repository.Add(_newTask);

    return new AppTaskDto
    {
      UserName = string.Empty,
      Title = _newTask.Title,
      Description = _newTask.Description,
      DueDate = _newTask.DueDate,
      Status = _newTask.Status,
      SubTask = _newTask.SubTasks.Select(s => s.Title).ToList(),
      Tags = _newTask.Tags.Select(t => t.Name).ToList()
    };
  }

  public async Task DeleteTask(int id)
  {
    await _repository.Delete(id);
  }

  public async Task<IEnumerable<AppTaskDto>> GetAllTask(string userId)
  {
    var tasks = await _repository.GetAllByUserId(userId);
    return tasks.Select(task => new AppTaskDto
    {
      UserName = task.User.UserName!,
      Title = task.Title,
      Description = task.Description,
      DueDate = task.DueDate,
      Status = task.Status,
      CategoryName = task.Category?.Name,
      SubTask = task.SubTasks.Select(s => s.Title).ToList(),
      Tags = task.Tags.Select(t => t.Name).ToList()
    });
  }

  public async Task<AppTaskDto?> GetTaskById(int id)
  {
    var task = await _repository.GetById(id);

    if(task != null)
      return new AppTaskDto
      {
        UserName = task.User.UserName!,
        Title = task.Title,
        Description = task.Description,
        DueDate = task.DueDate,
        Status = task.Status,
        CategoryName = task.Category?.Name,
        SubTask = task.SubTasks.Select(s => s.Title).ToList(),
        Tags = task.Tags.Select(t => t.Name).ToList()
      };

    else
      return null;
  }

  public async Task UpdateTask(UpdateAppTaskDto uptTask, int id)
  {
    AppTask? task = await _repository.GetById(id);
    if(task != null)
    {
      if(uptTask.Title != null)
        task.Title = uptTask.Title;
      if(uptTask.Description != null)
        task.Description = uptTask.Description;
      if(uptTask.DueDate != null)
        task.DueDate = uptTask.DueDate;
      if(uptTask.Status != null)
        task.Status = uptTask.Status;
      /*
      if(uptTask.Category != null)
        task.Category = uptTask.Category
      if(uptTask.SubTasks != null)
        task.SubTasks = uptTask.SubTasks
      if(uptTask.Tags != null)
        task.Tags = uptTask.Tags
      */
      await _repository.Update(task);
    }
  }
}
