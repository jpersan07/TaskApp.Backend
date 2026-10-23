using Moq;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Tests.Services;

public class SubTaskServiceTests
{
    private readonly Mock<ISubTaskRepository> _mockRepo;
    private readonly SubTaskService _service;

    public SubTaskServiceTests()
    {
        _mockRepo = new Mock<ISubTaskRepository>();
        _service = new SubTaskService(_mockRepo.Object);
    }

    private static SubTask BuildSubTask(int id = 1, int taskId = 10) => new SubTask
    {
        Id = id,
        Title = "Sub Task",
        TaskId = taskId,
        IsCompleted = false,
        Task = new AppTask { Id = taskId, Title = "Parent Task" }
    };

    [Fact]
    public async Task GetAllSubTasks_ReturnsCorrectNumberOfDtos()
    {
        var subs = new List<SubTask> { BuildSubTask(1, 10), BuildSubTask(2, 10) };
        _mockRepo.Setup(r => r.GetAllByTaskId(10)).ReturnsAsync(subs);

        var result = await _service.GetAllSubTasks(10);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllSubTasks_MapsSubTaskFieldsCorrectly()
    {
        var sub = BuildSubTask(1, 10);
        sub.Title = "Write tests";
        sub.IsCompleted = true;
        _mockRepo.Setup(r => r.GetAllByTaskId(10)).ReturnsAsync(new List<SubTask> { sub });

        var result = await _service.GetAllSubTasks(10);
        var dto = result.First();

        Assert.Equal(1, dto.Id);
        Assert.Equal("Write tests", dto.Title);
        Assert.Equal(10, dto.TaskId);
        Assert.True(dto.IsCompleted);
    }

    [Fact]
    public async Task GetSubTaskById_WhenSubTaskExists_ReturnsMappedDto()
    {
        var sub = BuildSubTask(1, 10);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(sub);

        var result = await _service.GetSubTaskById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Sub Task", result.Title);
        Assert.Equal(10, result.TaskId);
    }

    [Fact]
    public async Task GetSubTaskById_WhenSubTaskNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((SubTask?)null);

        var result = await _service.GetSubTaskById(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateSubTask_CallsRepositoryAddOnce()
    {
        var dto = new CreateSubTaskDto { Title = "New Sub", TaskId = 10, IsCompleted = false };

        await _service.CreateSubTask(dto);

        _mockRepo.Verify(r => r.Add(It.Is<SubTask>(s =>
            s.Title == "New Sub" &&
            s.TaskId == 10 &&
            s.IsCompleted == false
        )), Times.Once);
    }

    [Fact]
    public async Task CreateSubTask_ReturnsDtoWithCorrectFields()
    {
        var dto = new CreateSubTaskDto { Title = "Deploy app", TaskId = 5, IsCompleted = false };

        var result = await _service.CreateSubTask(dto);

        Assert.Equal("Deploy app", result.Title);
        Assert.Equal(5, result.TaskId);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task UpdateSubTask_WhenSubTaskExists_UpdatesTitleAndIsCompleted()
    {
        var sub = BuildSubTask(1, 10);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(sub);

        var uptDto = new UpdateSubTaskDto { Title = "Updated Sub", IsCompleted = true };
        await _service.UpdateSubTask(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<SubTask>(s =>
            s.Title == "Updated Sub" && s.IsCompleted == true
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateSubTask_WhenTitleIsNull_DoesNotOverwriteTitle()
    {
        var sub = BuildSubTask(1, 10);
        sub.Title = "Original Title";
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(sub);

        var uptDto = new UpdateSubTaskDto { Title = null, IsCompleted = true };
        await _service.UpdateSubTask(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<SubTask>(s => s.Title == "Original Title")), Times.Once);
    }

    [Fact]
    public async Task UpdateSubTask_WhenSubTaskNotFound_DoesNotCallUpdate()
    {
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((SubTask?)null);

        await _service.UpdateSubTask(new UpdateSubTaskDto { Title = "X" }, 99);

        _mockRepo.Verify(r => r.Update(It.IsAny<SubTask>()), Times.Never);
    }

    [Fact]
    public async Task DeleteSubTask_CallsRepositoryDeleteWithCorrectId()
    {
        await _service.DeleteSubTask(4);

        _mockRepo.Verify(r => r.Delete(4), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_DeletesEachSubTaskFromParentTask()
    {
        var subs = new List<SubTask> { BuildSubTask(1, 10), BuildSubTask(2, 10) };
        _mockRepo.Setup(r => r.GetAllByTaskId(10)).ReturnsAsync(subs);

        await _service.BulkDelete(10);

        _mockRepo.Verify(r => r.Delete(1), Times.Once);
        _mockRepo.Verify(r => r.Delete(2), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_WhenNoSubTasks_DoesNotCallDelete()
    {
        _mockRepo.Setup(r => r.GetAllByTaskId(10)).ReturnsAsync(new List<SubTask>());

        await _service.BulkDelete(10);

        _mockRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }
}
