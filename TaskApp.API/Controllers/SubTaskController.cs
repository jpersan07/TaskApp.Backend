using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class SubTaskController : ControllerBase
  {
    private readonly ISubTaskService _subTaskService;
    public SubTaskController(ISubTaskService subTaskService) => _subTaskService = subTaskService;

    /// <summary>
    /// A method to get all the SubTasks belonging to a Task
    /// </summary>
    /// <param name="taskId">The ID of the parent task</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(int taskId)
    {
      var result = await _subTaskService.GetAllSubTasks(taskId);
      return Ok(result);
    }

    /// <summary>
    /// A method to get the SubTask info by its ID
    /// </summary>
    /// <param name="id">The ID of the subtask you want to get the info from</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubTaskById(int id)
    {
      var result = await _subTaskService.GetSubTaskById(id);
      if (result != null)
        return Ok(result);
      else
        return NotFound();
    }

    /// <summary>
    /// A method to create a subtask linked to a parent task
    /// </summary>
    /// <param name="newSubTask">The SubTask obj that is going to be created</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateSubTask(CreateSubTaskDto newSubTask)
    {
      var result = await _subTaskService.CreateSubTask(newSubTask);
      return CreatedAtAction(nameof(GetSubTaskById), new { id = result.Id}, result);
    }

    /// <summary>
    /// A method to update a subtask by a new SubTask obj and its id
    /// </summary>
    /// <param name="uptSubTask">The new SubTask properties in a new obj</param>
    /// <param name="id">The id of the subtask wanting to update</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubTask(UpdateSubTaskDto uptSubTask, int id)
    {
      await _subTaskService.UpdateSubTask(uptSubTask, id);
      return NoContent();
    }

    /// <summary>
    /// A method to delete a subtask by its id
    /// </summary>
    /// <param name="id">The id of the subtask</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
      await _subTaskService.DeleteSubTask(id);
      return NoContent();
    }

    /// <summary>
    /// A method to delete all subtasks belonging to a parent task
    /// </summary>
    /// <param name="taskId">The id of the parent task</param>
    /// <returns></returns>
    [HttpDelete("bulk/{taskId}")]
    public async Task<IActionResult> BulkDelteUserId(int taskId)
    {
      await _subTaskService.BulkDelete(taskId);
      return NoContent();
    }
  }
}
