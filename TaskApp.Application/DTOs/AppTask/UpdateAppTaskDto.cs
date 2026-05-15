using TaskStatus = TaskApp.Domain.Enums.TaskStatus;


namespace TaskApp.Application.DTOs;

public class UpdateAppTaskDto
{
  public string? Title { get; set; }
  public string? Description { get; set; } 
  public DateTime? DueDate { get; set; } 
  public TaskStatus? Status { get; set; } 
  public string? Category { get; set; } 
  public ICollection<string>? SubTasks { get; set; } 
  public ICollection<string>? Tags { get; set; } 
}
