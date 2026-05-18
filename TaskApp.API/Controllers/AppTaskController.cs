using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppTaskController : ControllerBase
{
  private readonly IAppTaskService _taskService;
  public AppTaskController(IAppTaskService taskService) => _taskService = taskService;

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var result = await _taskService.GetAllTask(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    return Ok(result);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await _taskService.GetTaskById(id);

    if(result != null)
      return Ok(result);
    else
      return NotFound();
  }

  [HttpPost]
  public async Task<IActionResult> CreateTask(CreateAppTaskDto newTask)
  {
    var result = await _taskService.CreateTask(newTask, User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    return CreatedAtAction(nameof(GetById), new { id = result.Id}, result);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTask(UpdateAppTaskDto uptTask, int id)
  {
    await _taskService.UpdateTask(uptTask, id);
      return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTask(int id)
  {
    await _taskService.DeleteTask(id);
      return NoContent();
  }
  
}

