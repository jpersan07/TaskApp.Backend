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

  public async Task BulkDelete(string userId)
  {
    try
    {
      var allTasks = await _repository.GetAllByUserId(userId);
      foreach (var item in allTasks)
        await _repository.Delete(item.Id);
    }
    catch (Exception ex)
    {
      throw new Exception($"Error deleting all tasks for user '{userId}'", ex);
    }
  }

  public async Task<AppTaskDto> CreateTask(CreateAppTaskDto newTask, string userId)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception($"Error creating a new task '{newTask.Title}'", ex);
    }
  }

  public async Task DeleteTask(int id)
  {
    try
    {
      await _repository.Delete(id);
    }
    catch (Exception ex)
    {
      throw new Exception($"Error deleting the task with id '{id}'", ex);
    }
  }

  public async Task<IEnumerable<AppTaskDto>> GetAllTask(string userId)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception($"Error retrieving all tasks for user '{userId}'", ex);
    }
  }

  public async Task<AppTaskDto?> GetTaskById(int id)
  {
    try
    {
      var task = await _repository.GetById(id);

      if (task != null)
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
    catch (Exception ex)
    {
      throw new Exception($"Error retrieving the task with id '{id}'", ex);
    }
  }

  public async Task UpdateTask(UpdateAppTaskDto uptTask, int id)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception($"Error updating the task with id '{id}'", ex);
    }
  }
}
