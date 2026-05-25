using TaskApp.Application.DTOs;

namespace TaskApp.Application.Services;

public interface IAuthService
{
  public Task<AuthResponseDto> Login(LoginDto login);
  public Task<IEnumerable<string>> Register(RegisterDto register);
}
