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

  /// <summary>
  /// A method to get all the Tasks from the JWT user
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var result = await _taskService.GetAllTask(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    return Ok(result);
  }
  /// <summary>
  /// A method to get the Task info by its ID
  /// </summary>
  /// <param name="id">The ID of the task you want to get the info from</param>
  /// <returns></returns>
  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await _taskService.GetTaskById(id);

    if(result != null)
      return Ok(result);
    else
      return NotFound();
  }
  /// <summary>
  /// A method to create a task by a new Task obj
  /// </summary>
  /// <param name="newTask">The Task obj that is going to replace the previous task</param>
  /// <returns></returns>
  [HttpPost]
  public async Task<IActionResult> CreateTask(CreateAppTaskDto newTask)
  {
    //if(newTask.DueDate > DateTime.Now)
    var result = await _taskService.CreateTask(newTask, User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    return CreatedAtAction(nameof(GetById), new { id = result.Id}, result);
  }
  /// <summary>
  /// A method to update a task by a new Task obj and its id
  /// </summary>
  /// <param name="uptTask">The new Task properties in a new obj</param>
  /// <param name="id">The id of the task wanting to update</param>
  /// <returns></returns>
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTask(UpdateAppTaskDto uptTask, int id)
  {
    await _taskService.UpdateTask(uptTask, id);
      return NoContent();
  }
  /// <summary>
  /// A method to delete a task getting its id
  /// </summary>
  /// <param name="id">The id of the task</param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTask(int id)
  {
    await _taskService.DeleteTask(id);
      return NoContent();
  }
  
}

