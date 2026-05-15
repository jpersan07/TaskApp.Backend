using TaskApp.Application.DTOs;

namespace TaskApp.Application.Services;

public interface IAuthService
{
  public Task<AuthResponseDto> Login(LoginDto login);
  public Task<bool> Register(RegisterDto register);
}
