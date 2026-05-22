using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApp.API.Controllers;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Tests.Controllers;

public class AppTaskControllerTests
{
    private readonly Mock<IAppTaskService> _mockService;
    private readonly AppTaskController _controller;

    public AppTaskControllerTests()
    {
        _mockService = new Mock<IAppTaskService>();
        _controller = new AppTaskController(_mockService.Object);
        SetUser("user1");
    }

    private void SetUser(string userId)
    {
        var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
        var identity = new ClaimsIdentity(claims);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    private static AppTaskDto BuildDto(int id = 1) => new AppTaskDto
    {
        Id = id,
        Title = "Task " + id,
        Status = TaskStatus.NonStarted
    };

    [Fact]
    public async Task GetAll_ReturnsOkWithTaskList()
    {
        var tasks = new List<AppTaskDto> { BuildDto(1), BuildDto(2) };
        _mockService.Setup(s => s.GetAllTask("user1")).ReturnsAsync(tasks);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tasks, ok.Value);
    }

    [Fact]
    public async Task GetAll_CallsServiceWithCorrectUserId()
    {
        _mockService.Setup(s => s.GetAllTask("user1")).ReturnsAsync(new List<AppTaskDto>());

        await _controller.GetAll();

        _mockService.Verify(s => s.GetAllTask("user1"), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenTaskFound_ReturnsOkWithDto()
    {
        var dto = BuildDto(1);
        _mockService.Setup(s => s.GetTaskById(1)).ReturnsAsync(dto);

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetById_WhenTaskNotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetTaskById(99)).ReturnsAsync((AppTaskDto?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedAtActionWithCorrectIdAndDto()
    {
        var createDto = new CreateAppTaskDto { Title = "New Task" };
        var createdDto = BuildDto(5);
        _mockService.Setup(s => s.CreateTask(createDto, "user1")).ReturnsAsync(createdDto);

        var result = await _controller.CreateTask(createDto);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdAt.ActionName);
        Assert.Equal(5, createdAt.RouteValues!["id"]);
        Assert.Equal(createdDto, createdAt.Value);
    }

    [Fact]
    public async Task CreateTask_CallsServiceWithCorrectUserIdAndDto()
    {
        var createDto = new CreateAppTaskDto { Title = "Task" };
        _mockService.Setup(s => s.CreateTask(createDto, "user1")).ReturnsAsync(BuildDto(1));

        await _controller.CreateTask(createDto);

        _mockService.Verify(s => s.CreateTask(createDto, "user1"), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNoContent()
    {
        var uptDto = new UpdateAppTaskDto { Title = "Updated" };

        var result = await _controller.UpdateTask(uptDto, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTask_CallsServiceWithCorrectIdAndDto()
    {
        var uptDto = new UpdateAppTaskDto { Title = "Updated" };

        await _controller.UpdateTask(uptDto, 7);

        _mockService.Verify(s => s.UpdateTask(uptDto, 7), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContent()
    {
        var result = await _controller.DeleteTask(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTask_CallsServiceWithCorrectId()
    {
        await _controller.DeleteTask(3);

        _mockService.Verify(s => s.DeleteTask(3), Times.Once);
    }
}
