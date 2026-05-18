namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO with the required fields to create a new category
/// </summary>
public class CreateCategoryDto
{
  public string Name { get; set; } = string.Empty;
}
