using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuthService _service;
    public AuthController(IAuthService service) => _service = service;

    /// <summary>
    /// A method to register a new user
    /// </summary>
    /// <param name="register">The user credentials needed to create the account</param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto register)
    {
      if(await _service.Register(register))
        return Ok();
      else
        return BadRequest();
    }

    /// <summary>
    /// A method to log in and get a JWT token
    /// </summary>
    /// <param name="login">The user credentials needed to authenticate</param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto login)
    {
      var result = await _service.Login(login);
      if(result != null)
        return Ok(result);
      else
        return Unauthorized();
    }
  }
}
