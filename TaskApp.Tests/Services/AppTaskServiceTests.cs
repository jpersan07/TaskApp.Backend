using Moq;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Tests.Services;

public class AppTaskServiceTests
{
    private readonly Mock<IAppTaskRepository> _mockRepo;
    private readonly AppTaskService _service;

    public AppTaskServiceTests()
    {
        _mockRepo = new Mock<IAppTaskRepository>();
        _service = new AppTaskService(_mockRepo.Object);
    }

    private static AppTask BuildTask(int id = 1, string userId = "user1") => new AppTask
    {
        Id = id,
        Title = "Test Task",
        Description = "Test Description",
        UserId = userId,
        Status = TaskStatus.NonStarted,
        User = new AppUser { UserName = "testuser" },
        SubTasks = [],
        Tags = []
    };

    [Fact]
    public async Task GetAllTask_ReturnsCorrectNumberOfDtos()
    {
        var tasks = new List<AppTask> { BuildTask(1), BuildTask(2) };
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(tasks);

        var result = await _service.GetAllTask("user1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllTask_MapsTitleAndStatusCorrectly()
    {
        var task = BuildTask(1);
        task.Title = "My Task";
        task.Status = TaskStatus.InProgress;
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(new List<AppTask> { task });

        var result = await _service.GetAllTask("user1");
        var dto = result.First();

        Assert.Equal("My Task", dto.Title);
        Assert.Equal(TaskStatus.InProgress, dto.Status);
    }

    [Fact]
    public async Task GetAllTask_WhenRepositoryThrows_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _service.GetAllTask("user1"));
    }

    [Fact]
    public async Task GetTaskById_WhenTaskExists_ReturnsMappedDto()
    {
        var task = BuildTask(1);
        _mockRepo.Setup(r => r.GetTaskById(1)).ReturnsAsync(task);

        var result = await _service.GetTaskById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(task.Title, result.Title);
        Assert.Equal(task.Status, result.Status);
    }

    [Fact]
    public async Task GetTaskById_WhenTaskNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetTaskById(99)).ReturnsAsync((AppTask?)null);

        var result = await _service.GetTaskById(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTaskById_WhenRepositoryThrows_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetTaskById(1)).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _service.GetTaskById(1));
    }

    [Fact]
    public async Task CreateTask_CallsRepositoryAddOnce()
    {
        var dto = new CreateAppTaskDto { Title = "New Task", Description = "Desc" };

        await _service.CreateTask(dto, "user1");

        _mockRepo.Verify(r => r.Add(It.Is<AppTask>(t =>
            t.Title == "New Task" &&
            t.Description == "Desc" &&
            t.UserId == "user1"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateTask_ReturnsDtoWithCorrectTitle()
    {
        var dto = new CreateAppTaskDto { Title = "New Task" };

        var result = await _service.CreateTask(dto, "user1");

        Assert.Equal("New Task", result.Title);
    }

    [Fact]
    public async Task UpdateTask_WhenTaskExists_UpdatesFieldsAndCallsUpdate()
    {
        var task = BuildTask(1);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(task);

        var uptDto = new UpdateAppTaskDto { Title = "Updated Title", Status = TaskStatus.InProgress };
        await _service.UpdateTask(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<AppTask>(t =>
            t.Title == "Updated Title" &&
            t.Status == TaskStatus.InProgress
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_WhenTitleIsNull_DoesNotOverwriteTitle()
    {
        var task = BuildTask(1);
        task.Title = "Original Title";
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(task);

        var uptDto = new UpdateAppTaskDto { Title = null, Status = TaskStatus.Paused };
        await _service.UpdateTask(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<AppTask>(t => t.Title == "Original Title")), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_WhenTaskNotFound_DoesNotCallUpdate()
    {
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((AppTask?)null);

        await _service.UpdateTask(new UpdateAppTaskDto { Title = "X" }, 99);

        _mockRepo.Verify(r => r.Update(It.IsAny<AppTask>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTask_CallsRepositoryDeleteWithCorrectId()
    {
        await _service.DeleteTask(5);

        _mockRepo.Verify(r => r.Delete(5), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_DeletesEachTaskFromUser()
    {
        var tasks = new List<AppTask> { BuildTask(1), BuildTask(2), BuildTask(3) };
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(tasks);

        await _service.BulkDelete("user1");

        _mockRepo.Verify(r => r.Delete(1), Times.Once);
        _mockRepo.Verify(r => r.Delete(2), Times.Once);
        _mockRepo.Verify(r => r.Delete(3), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_WhenNoTasks_DoesNotCallDelete()
    {
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(new List<AppTask>());

        await _service.BulkDelete("user1");

        _mockRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }
}
