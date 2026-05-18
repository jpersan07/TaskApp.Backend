namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO representing a subtask returned to the client
/// </summary>
public class SubTaskDto
{
  public int Id { get; set; } 
  public string Title { get; set; } = string.Empty;
  public int TaskId { get; set; }
  public bool IsCompleted { get; set; } = false;
}
