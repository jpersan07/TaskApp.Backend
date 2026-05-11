using System;
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
    return await _context.Set<SubTask>().Where(x => x.TaskId == taskId).ToListAsync();
  }
}
