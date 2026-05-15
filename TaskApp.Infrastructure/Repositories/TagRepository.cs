using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Infrastructure.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
  public TagRepository(AppDbContext context) : base(context)
  {
  }

  public async Task<IEnumerable<Tag>> GetAllByUserId(string userId)
  {
    try
    {
      return await _context.Set<Tag>().Where(x => x.UserId == userId).ToListAsync();
    }
    catch (DbException ex)
    {
      throw new Exception($"Error getting all Tags from user '{userId}' from database", ex);
    }
  }
}
