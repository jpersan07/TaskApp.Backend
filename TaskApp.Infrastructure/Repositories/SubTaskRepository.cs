using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Infrastructure.Repositories;


public class SubTaskRepository : Repository<SubTask>, ISubTaskRepository
{
  public SubTaskRepository(AppDbContext context) : base(context)
  {
  }

  /// <summary>
  /// A method to get all subtasks belonging to a parent task
  /// </summary>
  /// <param name="taskId">The id of the parent task whose subtasks are going to be retrieved</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<IEnumerable<SubTask>> GetAllByTaskId(int taskId)
  {
    try
    {
      return await _context.Set<SubTask>().Where(x => x.TaskId == taskId).ToListAsync();
    }
    catch (DbException ex)
    {
      throw new Exception($"Error getting all subtasks from Task '{taskId}' from database", ex);
    }
  }
}
