using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApp.API.Controllers;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.Tests.Controllers;

public class SubTaskControllerTests
{
    private readonly Mock<ISubTaskService> _mockService;
    private readonly SubTaskController _controller;

    public SubTaskControllerTests()
    {
        _mockService = new Mock<ISubTaskService>();
        _controller = new SubTaskController(_mockService.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private static SubTaskDto BuildDto(int id = 1, int taskId = 10) => new SubTaskDto
    {
        Id = id,
        Title = "SubTask " + id,
        TaskId = taskId,
        IsCompleted = false
    };

    [Fact]
    public async Task GetAll_ReturnsOkWithSubTaskList()
    {
        var subs = new List<SubTaskDto> { BuildDto(1, 10), BuildDto(2, 10) };
        _mockService.Setup(s => s.GetAllSubTasks(10)).ReturnsAsync(subs);

        var result = await _controller.GetAll(10);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(subs, ok.Value);
    }

    [Fact]
    public async Task GetAll_CallsServiceWithCorrectTaskId()
    {
        _mockService.Setup(s => s.GetAllSubTasks(10)).ReturnsAsync(new List<SubTaskDto>());

        await _controller.GetAll(10);

        _mockService.Verify(s => s.GetAllSubTasks(10), Times.Once);
    }

    [Fact]
    public async Task GetSubTaskById_WhenFound_ReturnsOkWithDto()
    {
        var dto = BuildDto(1, 10);
        _mockService.Setup(s => s.GetSubTaskById(1)).ReturnsAsync(dto);

        var result = await _controller.GetSubTaskById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetSubTaskById_WhenNotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetSubTaskById(99)).ReturnsAsync((SubTaskDto)null!);

        var result = await _controller.GetSubTaskById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateSubTask_ReturnsCreatedAtActionWithCorrectIdAndDto()
    {
        var createDto = new CreateSubTaskDto { Title = "New Sub", TaskId = 10 };
        var createdDto = BuildDto(5, 10);
        _mockService.Setup(s => s.CreateSubTask(createDto)).ReturnsAsync(createdDto);

        var result = await _controller.CreateSubTask(createDto);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetSubTaskById), createdAt.ActionName);
        Assert.Equal(5, createdAt.RouteValues!["id"]);
        Assert.Equal(createdDto, createdAt.Value);
    }

    [Fact]
    public async Task CreateSubTask_CallsServiceWithCorrectDto()
    {
        var createDto = new CreateSubTaskDto { Title = "Deploy", TaskId = 10 };
        _mockService.Setup(s => s.CreateSubTask(createDto)).ReturnsAsync(BuildDto(1));

        await _controller.CreateSubTask(createDto);

        _mockService.Verify(s => s.CreateSubTask(createDto), Times.Once);
    }

    [Fact]
    public async Task UpdateSubTask_ReturnsNoContent()
    {
        var uptDto = new UpdateSubTaskDto { Title = "Updated" };

        var result = await _controller.UpdateSubTask(uptDto, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateSubTask_CallsServiceWithCorrectIdAndDto()
    {
        var uptDto = new UpdateSubTaskDto { Title = "Updated" };

        await _controller.UpdateSubTask(uptDto, 9);

        _mockService.Verify(s => s.UpdateSubTask(uptDto, 9), Times.Once);
    }

    [Fact]
    public async Task DeleteById_ReturnsNoContent()
    {
        var result = await _controller.DeleteById(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteById_CallsServiceWithCorrectId()
    {
        await _controller.DeleteById(2);

        _mockService.Verify(s => s.DeleteSubTask(2), Times.Once);
    }

    [Fact]
    public async Task BulkDelteUserId_ReturnsNoContent()
    {
        var result = await _controller.BulkDelteUserId(10);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task BulkDelteUserId_CallsServiceWithCorrectTaskId()
    {
        await _controller.BulkDelteUserId(10);

        _mockService.Verify(s => s.BulkDelete(10), Times.Once);
    }
}
