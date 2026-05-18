using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure;

/// <summary>
/// The Entity Framework database context for the application, extending IdentityDbContext to include user authentication
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser>
{
  /// <summary>The tasks table</summary>
  public DbSet<AppTask> AppTasks { get; set; }
  /// <summary>The categories table</summary>
  public DbSet<Category> Categories { get; set; }
  /// <summary>The tags table</summary>
  public DbSet<Tag> Tags { get; set; }
  /// <summary>The subtasks table</summary>
  public DbSet<SubTask> SubTasks { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

  /// <summary>
  /// Configures the many-to-many relationship between AppTask and Tag using a join table
  /// </summary>
  /// <param name="modelBuilder">The builder used to construct the model for the database</param>
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