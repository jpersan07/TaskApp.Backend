using Moq;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepo;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepo = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepo.Object);
    }

    private static Category BuildCategory(int id = 1, string userId = "user1") => new Category
    {
        Id = id,
        Name = "Work",
        Color = "#FF0000",
        UserId = userId,
        User = new AppUser { UserName = "testuser" },
        Tasks = []
    };

    [Fact]
    public async Task GetAllCat_ReturnsCorrectNumberOfDtos()
    {
        var cats = new List<Category> { BuildCategory(1), BuildCategory(2) };
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(cats);

        var result = await _service.GetAllCat("user1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllCat_MapsCategoryFieldsCorrectly()
    {
        var cat = BuildCategory(1);
        cat.Name = "Personal";
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(new List<Category> { cat });

        var result = await _service.GetAllCat("user1");
        var dto = result.First();

        Assert.Equal(1, dto.Id);
        Assert.Equal("Personal", dto.Name);
        Assert.Equal("user1", dto.UserId);
    }

    [Fact]
    public async Task GetAllCat_WhenRepositoryThrows_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _service.GetAllCat("user1"));
    }

    [Fact]
    public async Task GetCatById_WhenCategoryExists_ReturnsMappedDto()
    {
        var cat = BuildCategory(1);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(cat);

        var result = await _service.GetCatById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Work", result.Name);
    }

    [Fact]
    public async Task GetCatById_WhenCategoryNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((Category?)null);

        var result = await _service.GetCatById(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCatById_WhenRepositoryThrows_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetById(1)).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _service.GetCatById(1));
    }

    [Fact]
    public async Task CreateCategory_CallsRepositoryAddOnce()
    {
        var dto = new CreateCategoryDto { Name = "Health" };

        await _service.CreateCategory(dto, "user1");

        _mockRepo.Verify(r => r.Add(It.Is<Category>(c =>
            c.Name == "Health" && c.UserId == "user1"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateCategory_ReturnsDtoWithCorrectFields()
    {
        var dto = new CreateCategoryDto { Name = "Finance" };

        var result = await _service.CreateCategory(dto, "user1");

        Assert.Equal("Finance", result.Name);
        Assert.Equal("user1", result.UserId);
    }

    [Fact]
    public async Task UpdateCat_WhenCategoryExists_UpdatesNameAndCallsUpdate()
    {
        var cat = BuildCategory(1);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(cat);

        var uptDto = new UpdateCategoryDto { Name = "Updated Name" };
        await _service.UpdateCat(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<Category>(c => c.Name == "Updated Name")), Times.Once);
    }

    [Fact]
    public async Task UpdateCat_WhenNameIsNull_DoesNotOverwriteName()
    {
        var cat = BuildCategory(1);
        cat.Name = "Original Name";
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(cat);

        var uptDto = new UpdateCategoryDto { Name = null };
        await _service.UpdateCat(uptDto, 1);

        _mockRepo.Verify(r => r.Update(It.Is<Category>(c => c.Name == "Original Name")), Times.Once);
    }

    [Fact]
    public async Task UpdateCat_WhenCategoryNotFound_DoesNotCallUpdate()
    {
        _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((Category?)null);

        await _service.UpdateCat(new UpdateCategoryDto { Name = "X" }, 99);

        _mockRepo.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCat_CallsRepositoryDeleteWithCorrectId()
    {
        await _service.DeleteCat(3);

        _mockRepo.Verify(r => r.Delete(3), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_DeletesEachCategoryFromUser()
    {
        var cats = new List<Category> { BuildCategory(1), BuildCategory(2) };
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(cats);

        await _service.BulkDelete("user1");

        _mockRepo.Verify(r => r.Delete(1), Times.Once);
        _mockRepo.Verify(r => r.Delete(2), Times.Once);
    }

    [Fact]
    public async Task BulkDelete_WhenNoCategories_DoesNotCallDelete()
    {
        _mockRepo.Setup(r => r.GetAllByUserId("user1")).ReturnsAsync(new List<Category>());

        await _service.BulkDelete("user1");

        _mockRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }
}
