using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SubTaskController : ControllerBase
  {
    private readonly ISubTaskService _subTaskService;
    public SubTaskController(ISubTaskService subTaskService) => _subTaskService = subTaskService;

    [HttpGet]
    public async Task<IActionResult> GetAll(int taskId)
    {
      var result = await _subTaskService.GetAllSubTasks(taskId);
      return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubTaskById(int id)
    {
      var result = await _subTaskService.GetSubTaskById(id);
      if (result != null)
        return Ok(result);
      else
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubTask(CreateSubTaskDto newSubTask)
    {
      var result = await _subTaskService.CreateSubTask(newSubTask);
      return CreatedAtAction(nameof(GetSubTaskById), new { id = result.Id}, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubTask(UpdateSubTaskDto uptSubTask, int id)
    {
      await _subTaskService.UpdateSubTask(uptSubTask, id);
      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(int id)
    {
      await _subTaskService.DeleteSubTask(id);
      return NoContent();
    }

    [HttpDelete("bulk/{taskId}")]
    public async Task<IActionResult> BulkDelteUserId(int taskId)
    {
      await _subTaskService.BulkDelete(taskId);
      return NoContent();
    }
  }
}
