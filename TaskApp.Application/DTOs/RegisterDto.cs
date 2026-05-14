using System;
using System.ComponentModel.DataAnnotations;

namespace TaskApp.Application.DTOs;

public class RegisterDto
{
  [Required]
  public string Email { get; set; } = string.Empty;  

  [Required]
  public string UserName { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; }  = string.Empty;
}
