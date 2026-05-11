using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Infrastructure.Repositories;

public class AppTaskRepository : Repository<AppTask>, IAppTaskRepository
{
  public AppTaskRepository(AppDbContext context) : base(context)
  {
    
  }

  public async Task<IEnumerable<AppTask>> GetAllByUserId(string userId)
  {
    return await _context.Set<AppTask>().Where(x => x.UserId == userId).ToListAsync();
  }
}
