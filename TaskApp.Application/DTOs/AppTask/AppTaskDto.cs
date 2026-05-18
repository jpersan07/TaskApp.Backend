using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO representing a task returned to the client, including related data such as subtasks and tags
/// </summary>
public class AppTaskDto
{
  public int Id { get; set; }
  public string UserName { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? DueDate { get; set; }
  public TaskStatus? Status { get; set; } = TaskStatus.NonStarted;
  public string? CategoryName { get; set; }
  public ICollection<string>? SubTask { get; set; }
  public ICollection<string>? Tags { get; set; }
}
