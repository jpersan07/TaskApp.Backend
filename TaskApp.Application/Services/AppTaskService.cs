using System;
using Microsoft.EntityFrameworkCore;
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

  /// <summary>
  /// A method to Bulk delete tasks from an user
  /// </summary>
  /// <param name="userId">The user that is going to get all the task deleted</param>
  /// <returns>Returns an expcetion if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task BulkDelete(string userId)
  {
    var allTasks = await _repository.GetAllByUserId(userId);
    foreach (var item in allTasks)
      await _repository.Delete(item.Id);
  }

  /// <summary>
  /// A method to create a new task from a Task obj
  /// </summary>
  /// <param name="newTask">The task obj that is going to replace the old task</param>
  /// <param name="userId">The owner of the task</param>
  /// <returns>Returns an expcetion if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
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

  /// <summary>
  /// A method to delete a task by its id
  /// </summary>
  /// <param name="id">The id of the task that is going to be deleted</param>
  /// <returns>Returns an expcetion if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task DeleteTask(int id)
  {
    await _repository.Delete(id);
  }

  /// <summary>
  /// A method to get all user's task by its userId
  /// </summary>
  /// <param name="userId">The owner of all the task that are going to be getted</param>
  /// <returns>Returns an expcetion if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<IEnumerable<AppTaskDto>> GetAllTask(string userId)
  {
    var tasks = await _repository.GetAllByUserId(userId);
    return tasks.Select(task => new AppTaskDto
    {
      Id = task.Id,
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

  /// <summary>
  /// A method to get a task by its id
  /// </summary>
  /// <param name="id">The id of the task you want to retrieve</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<AppTaskDto?> GetTaskById(int id)
  {
    var task = await _repository.GetTaskById(id);

    if (task != null)
      return new AppTaskDto
      {
        Id = id,
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


  /// <summary>
  /// A method to update a task by a new Task obj and its id
  /// </summary>
  /// <param name="uptTask">The new Task properties in a new obj</param>
  /// <param name="id">The id of the task wanting to update</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task UpdateTask(UpdateAppTaskDto uptTask, int id)
  {
    AppTask? task = await _repository.GetById(id);
    if (task != null)
    {
      if (uptTask.Title != null)
        task.Title = uptTask.Title;
      if (uptTask.Description != null)
        task.Description = uptTask.Description;
      if (uptTask.DueDate != null)
        task.DueDate = uptTask.DueDate;
      if (uptTask.Status != null)
        task.Status = uptTask.Status;

      await _repository.Update(task);
    }
  }
}
