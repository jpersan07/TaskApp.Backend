using System;
using System.ComponentModel.DataAnnotations;

namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO with the data needed to register a new user
/// </summary>
public class RegisterDto
{
  [Required]
  public string Email { get; set; } = string.Empty;  

  [Required]
  public string UserName { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; }  = string.Empty;
}
