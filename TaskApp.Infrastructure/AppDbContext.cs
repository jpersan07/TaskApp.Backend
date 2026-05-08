using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUser>
{
  public DbSet<AppTask> AppTasks { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<Tag> Tags { get; set; }
  public DbSet<SubTask> SubTasks { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
}