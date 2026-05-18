using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Infrastructure.Repositories;

public class AppTaskRepository : Repository<AppTask>, IAppTaskRepository
{
  public AppTaskRepository(AppDbContext context) : base(context)
  {

  }

  /// <summary>
  /// A method to get all tasks belonging to a user
  /// </summary>
  /// <param name="userId">The id of the user whose tasks are going to be retrieved</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<IEnumerable<AppTask>> GetAllByUserId(string userId)
  {
    try
    {
      return await _context.Set<AppTask>()
      .Include(x => x.User)
      .Include(x => x.SubTasks)
      .Include(x => x.Tags)
      .Include(x => x.Category)
      .Where(x => x.UserId == userId).ToListAsync();
    }
    catch (DbException ex)
    {
      throw new Exception($"Error getting all Tasks from user '{userId}' from database", ex);
    }
  }

  public async Task<AppTask?> GetTaskById(int id)
  {
    try
    {
      return await _context.Set<AppTask>()
      .Include(x => x.User)
      .Include(x => x.SubTasks)
      .Include(x => x.Tags)
      .Include(x => x.Category)
      .Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    catch (DbException ex)
    {
      throw new Exception($"Error getting task '{id}' from database", ex);
    }
  }
}
