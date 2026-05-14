using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure.Services;


public class AuthService : IAuthService
{
  private readonly UserManager<AppUser> _userManager;
  private readonly SignInManager<AppUser> _signInManager;
  private readonly IConfiguration _configuration;

  public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _configuration = configuration;
  }

  public Task<AuthResponseDto> Login(LoginDto login)
  {
    throw new NotImplementedException();
  }

  public Task<bool> Register(RegisterDto register)
  {
    throw new NotImplementedException();
  }
}

