using TaskApp.Domain.Enums;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Domain.Entities;

public class AppTask
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? DueDate { get; set; }
  public TaskStatus? Status { get; set; } = TaskStatus.NonStarted;
  public string UserId { get; set; } = string.Empty;
  public AppUser User { get; set; } = null!;
  public int? CategoryId { get; set; }
  public Category? Category { get; set; }
  public ICollection<SubTask> SubTasks { get; set; } = [];
  public ICollection<Tag> Tags { get; set; } = [];
}