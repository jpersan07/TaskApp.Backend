namespace TaskApp.Domain.Entities;

public class Tag
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
  public AppUser User { get; set; } = null!;
  public ICollection<AppTask> Tasks { get; set; } = [];
}