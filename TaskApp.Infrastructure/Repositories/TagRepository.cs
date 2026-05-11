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

  public async Task<IEnumerable<Tag>> GetAllByUserId(string userId)
  {
    return await _context.Set<Tag>().Where(x => x.UserId == userId).ToListAsync();
  }
}
