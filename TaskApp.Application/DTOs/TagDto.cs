using System;

namespace TaskApp.Application.DTOs;

public class TagDto
{
  public int Id { get; set; } 
  public string Name { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
}
