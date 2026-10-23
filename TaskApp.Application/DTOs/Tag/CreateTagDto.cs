namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO with the required fields to create a new tag
/// </summary>
public class CreateTagDto
{
  public string Name { get; set; } = string.Empty;
}
