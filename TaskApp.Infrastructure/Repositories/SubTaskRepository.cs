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
