using System;
using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Infrastructure.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
  public TagRepository(AppDbContext context) : base(context)
  {
  }

  /// <summary>
  /// A method to get all tags belonging to a user
  /// </summary>
  /// <param name="userId">The id of the user whose tags are going to be retrieved</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<IEnumerable<Tag>> GetAllByUserId(string userId)
  {
    return await _context.Set<Tag>().Where(x => x.UserId == userId).ToListAsync();
  }
}
