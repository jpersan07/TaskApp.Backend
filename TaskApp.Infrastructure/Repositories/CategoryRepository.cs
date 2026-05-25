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

  /// <summary>
  /// A method to get all categories belonging to a user
  /// </summary>
  /// <param name="userId">The id of the user whose categories are going to be retrieved</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<IEnumerable<Category>> GetAllByUserId(string userId)
  {
    return await _context.Set<Category>().Where(x => x.UserId == userId).ToListAsync();
  }

  public async Task<Category?> GetByNameUserId(string name, string userId)
  {
    return await _context.Set<Category>().Where(x => x.UserId == userId && x.Name == name).FirstOrDefaultAsync();
  }
}
