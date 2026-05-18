using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure.Services;


/// <summary>
/// Service that handles user authentication: registration and JWT token generation on login
/// </summary>
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

  /// <summary>
  /// A method to authenticate a user and return a signed JWT token
  /// </summary>
  /// <param name="login">The credentials needed to authenticate the user</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<AuthResponseDto> Login(LoginDto login)
  {
    try
    {
      var user = await _userManager.FindByNameAsync(login.UserName);
      if (user == null)
        return null!;

      SignInResult userLoged = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
      if (!userLoged.Succeeded)
        return null!;

      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!)
      };

      SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
      SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: creds
      );

      return new AuthResponseDto { Token = new JwtSecurityTokenHandler().WriteToken(token) };
    }
    catch (Exception ex)
    {
      throw new Exception($"Error logging in user '{login.UserName}'", ex);
    }
  }

  /// <summary>
  /// A method to register a new user in the system
  /// </summary>
  /// <param name="register">The data needed to create the new user account</param>
  /// <returns>Returns an exception if the method cant be resolved</returns>
  /// <exception cref="Exception"></exception>
  public async Task<bool> Register(RegisterDto register)
  {
    try
    {
      AppUser user = new AppUser { Email = register.Email, UserName = register.UserName };
      IdentityResult userCreate = await _userManager.CreateAsync(user, register.Password);
      if (userCreate.Succeeded)
        return true;
      else
        return false;
    }
    catch (Exception ex)
    {
      throw new Exception($"Error registering the new user '{register.UserName}'", ex);
    }
  }
}

