using System;
using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
  public CategoryRepository(AppDbContext context) : base(context)
  {
  }

  public async Task<IEnumerable<Category>> GetAllByUserId(string userId)
  {
    return await _context.Set<Category>().Where(x => x.UserId == userId).ToListAsync();
  }
}
