namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO with the required fields to create a new task
/// </summary>
public class CreateAppTaskDto
{
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; } 
  public DateTime? DueDate { get; set; }
  public string? CategoryName { get; set; }
}
