using System.ComponentModel.DataAnnotations;

namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO with the credentials needed to authenticate a user
/// </summary>
public class LoginDto
{
  [Required]
  public string UserName { get; set; } = string.Empty;
  
  [Required]
  public string Password { get; set; } = string.Empty;
}
