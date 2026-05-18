namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO with optional fields to update an existing subtask
/// </summary>
public class UpdateSubTaskDto
{
  public string? Title { get; set; } 
  public bool? IsCompleted { get; set; } 
}
