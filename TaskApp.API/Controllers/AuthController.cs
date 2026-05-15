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

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto register)
    {
      if(await _service.Register(register))
        return Ok();
      else
        return BadRequest();
    }

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
