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

  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<AppTask>()
          .HasMany(t => t.Tags)
          .WithMany(t => t.Tasks)
          .UsingEntity(j => j.ToTable("AppTaskTag"));

      modelBuilder.Entity<Tag>()
          .HasMany(t => t.Tasks)
          .WithMany(t => t.Tags)
          .UsingEntity<Dictionary<string, object>>(
              "AppTaskTag",
              j => j.HasOne<AppTask>().WithMany().OnDelete(DeleteBehavior.Cascade),
              j => j.HasOne<Tag>().WithMany().OnDelete(DeleteBehavior.NoAction)
          );
  }

}