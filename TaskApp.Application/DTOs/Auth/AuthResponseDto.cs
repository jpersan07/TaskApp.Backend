using System.ComponentModel.DataAnnotations;

namespace TaskApp.Application.DTOs;

/// <summary>
/// DTO returned after a successful login containing the JWT token
/// </summary>
public class AuthResponseDto
{
  public string Token { get; set; } = string.Empty;
}
