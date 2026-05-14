using System.ComponentModel.DataAnnotations;

namespace TaskApp.Application.DTOs;

public class AuthResponseDto
{
  public string Token { get; set; } = string.Empty;
}
