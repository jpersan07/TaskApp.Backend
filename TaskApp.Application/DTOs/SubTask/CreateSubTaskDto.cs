namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO with the required fields to create a new subtask
/// </summary>
public class CreateSubTaskDto
{
  public string Title { get; set; } = string.Empty;
  public int TaskId { get; set; } 
  public bool IsCompleted { get; set; } = false;
}
