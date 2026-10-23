namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO representing a category returned to the client
/// </summary>
public class CategoryDto
{
  public int Id { get; set; } 
  public string Name { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
}
