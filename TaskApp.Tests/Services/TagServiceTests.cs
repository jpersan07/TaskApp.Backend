using Moq;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Tests.Services;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _mockRepo;
    private readonly TagService _service;

    public TagServiceTests()
    {
        _mockRepo = new Mock<ITagRepository>();
        _service = new TagService(_mockRepo.Object);
    }

    private static Tag BuildTag(int id = 1, string userId = "user1") => new Tag
    {
        Id = id,
        Name = "Urgent",
        UserId = userId,
        User = new AppUser { UserName = "testuser" },
        Tasks = []
    };

    [Fact]
    public async Task GetAllTags_ReturnsCorrectNumberOfDtos()
    {
        var tags = new List<Tag> { BuildTag(1), BuildTag(2) };
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(tags);

        var result = await _service.GetAllTags("user1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllTags_MapsTagFieldsCorrectly()
    {
        var tag = BuildTag(1);
        tag.Name = "Important";
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(new List<Tag> { tag });

        var result = await _service.GetAllTags("user1");
        var dto = result.First();

        Assert.Equal(1, dto.Id);
        Assert.Equal("Important", dto.Name);
        Assert.Equal("user1", dto.UserId);
    }

    [Fact]
    public async Task GetAllTags_WhenRepositoryThrows_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _service.GetAllTags("user1"));
    }

    [Fact]
    public async Task GetTagById_WhenTagExists_ReturnsMappedDto()
    {
        var tag = BuildTag(1);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(tag);

        var result = await _service.GetTagById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Urgent", result.Name);
    }

    [Fact]
    public async Task GetTagById_WhenTagNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((Tag?)null);

        var result = await _service.GetTagById(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTagById_WhenRepositoryThrows_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetById(1)).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _service.GetTagById(1));
    }

    [Fact]
    public async Task CreateTag_CallsRepositoryAddOnce()
    {
        var dto = new CreateTagDto { Name = "Bug" };

        await _service.CreateTag(dto, "user1");

        _mockRepo.Verify(r => r.Add(It.Is<Tag>(t =>
            t.Name == "Bug" && t.UserId == "user1"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateTag_ReturnsDtoWithCorrectFields()
    {
        var dto = new CreateTagDto { Name = "Feature" };

        var result = await _service.CreateTag(dto, "user1");

        Assert.Equal("Feature", result.Name);
        Assert.Equal("user1", result.UserId);
    }

    [Fact]
    public async Task UpdateTag_WhenTagExists_UpdatesNameAndCallsUpdate()
    {
        var tag = BuildTag(1);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(tag);

        var uptDto = new UpdateTagDto { Name = "Updated Tag" };
        await _service.UpdateTag(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<Tag>(t => t.Name == "Updated Tag")), Times.Once);
    }

    [Fact]
    public async Task UpdateTag_WhenNameIsNull_DoesNotOverwriteName()
    {
        var tag = BuildTag(1);
        tag.Name = "Original Name";
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(tag);

        var uptDto = new UpdateTagDto { Name = null };
        await _service.UpdateTag(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<Tag>(t => t.Name == "Original Name")), Times.Once);
    }

    [Fact]
    public async Task UpdateTag_WhenTagNotFound_DoesNotCallUpdate()
    {
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((Tag?)null);

        await _service.UpdateTag(new UpdateTagDto { Name = "X" }, 99);

        _mockRepo.Verify(r => r.Update(It.IsAny<Tag>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTag_CallsRepositoryDeleteWithCorrectId()
    {
        await _service.DeleteTag(7);

        _mockRepo.Verify(r => r.Delete(7), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_DeletesEachTagFromUser()
    {
        var tags = new List<Tag> { BuildTag(1), BuildTag(2) };
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(tags);

        await _service.BulkDelete("user1");

        _mockRepo.Verify(r => r.Delete(1), Times.Once);
        _mockRepo.Verify(r => r.Delete(2), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_WhenNoTags_DoesNotCallDelete()
    {
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(new List<Tag>());

        await _service.BulkDelete("user1");

        _mockRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task BulkDelete_WhenRepositoryThrows_WrapsException()
    {
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ThrowsAsync(new Exception("DB error"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.BulkDelete("user1"));
        Assert.Contains("Error deleting all tags", ex.Message);
    }

    [Fact]
    public async Task CreateTag_WhenRepositoryThrows_WrapsException()
    {
        _mockRepo.Setup(r => r.Add(It.IsAny<Tag>())).ThrowsAsync(new Exception("DB error"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateTag(new CreateTagDto { Name = "Urgent" }, "user1"));
        Assert.Contains("Error creating", ex.Message);
    }

    [Fact]
    public async Task DeleteTag_WhenRepositoryThrows_WrapsException()
    {
        _mockRepo.Setup(r => r.Delete(1)).ThrowsAsync(new Exception("DB error"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteTag(1));
        Assert.Contains("Error deleting the tag", ex.Message);
    }

    [Fact]
    public async Task UpdateTag_WhenRepositoryThrows_WrapsException()
    {
        _mockRepo.Setup(r => r.GetById(1)).ThrowsAsync(new Exception("DB error"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateTag(new UpdateTagDto { Name = "X" }, 1));
        Assert.Contains("Error updating the tag", ex.Message);
    }
}
