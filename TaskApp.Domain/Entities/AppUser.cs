using Microsoft.AspNetCore.Identity;

namespace TaskApp.Domain.Entities;

public class AppUser : IdentityUser
{
  public ICollection<AppTask> Tasks { get; set; }  = [];
  public ICollection<Category> Categories { get; set; } = [];
  public ICollection<Tag> Tags { get; set; } = [];
}