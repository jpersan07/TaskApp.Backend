using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApp.API.Controllers;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.Tests.Controllers;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mockService = new Mock<ICategoryService>();
        _controller = new CategoryController(_mockService.Object);
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

    private static CategoryDto BuildDto(int id = 1) => new CategoryDto
    {
        Id = id,
        Name = "Category " + id,
        UserId = "user1"
    };

    [Fact]
    public async Task GetAll_ReturnsOkWithCategoryList()
    {
        var cats = new List<CategoryDto> { BuildDto(1), BuildDto(2) };
        _mockService.Setup(s => s.GetAllCat("user1")).ReturnsAsync(cats);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cats, ok.Value);
    }

    [Fact]
    public async Task GetAll_CallsServiceWithCorrectUserId()
    {
        _mockService.Setup(s => s.GetAllCat("user1")).ReturnsAsync(new List<CategoryDto>());

        await _controller.GetAll();

        _mockService.Verify(s => s.GetAllCat("user1"), Times.Once);
    }

    [Fact]
    public async Task GetCategoryById_WhenFound_ReturnsOkWithDto()
    {
        var dto = BuildDto(1);
        _mockService.Setup(s => s.GetCatById(1)).ReturnsAsync(dto);

        var result = await _controller.GetCategoryById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetCategoryById_WhenNotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetCatById(99)).ReturnsAsync((CategoryDto?)null);

        var result = await _controller.GetCategoryById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedAtActionWithCorrectIdAndDto()
    {
        var createDto = new CreateCategoryDto { Name = "Work" };
        var createdDto = BuildDto(3);
        _mockService.Setup(s => s.CreateCategory(createDto, "user1")).ReturnsAsync(createdDto);

        var result = await _controller.CreateCategory(createDto);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetCategoryById), createdAt.ActionName);
        Assert.Equal(3, createdAt.RouteValues!["id"]);
        Assert.Equal(createdDto, createdAt.Value);
    }

    [Fact]
    public async Task CreateCategory_CallsServiceWithCorrectUserIdAndDto()
    {
        var createDto = new CreateCategoryDto { Name = "School" };
        _mockService.Setup(s => s.CreateCategory(createDto, "user1")).ReturnsAsync(BuildDto(1));

        await _controller.CreateCategory(createDto);

        _mockService.Verify(s => s.CreateCategory(createDto, "user1"), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsNoContent()
    {
        var uptDto = new UpdateCategoryDto { Name = "Updated" };

        var result = await _controller.UpdateCategory(uptDto, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateCategory_CallsServiceWithCorrectIdAndDto()
    {
        var uptDto = new UpdateCategoryDto { Name = "Updated" };

        await _controller.UpdateCategory(uptDto, 5);

        _mockService.Verify(s => s.UpdateCat(uptDto, 5), Times.Once);
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
        await _controller.DeleteById(4);

        _mockService.Verify(s => s.DeleteCat(4), Times.Once);
    }

    [Fact]
    public async Task BulkDelteUserId_ReturnsNoContent()
    {
        var result = await _controller.BulkDelteUserId();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task BulkDelteUserId_CallsServiceWithCorrectUserId()
    {
        await _controller.BulkDelteUserId();

        _mockService.Verify(s => s.BulkDelete("user1"), Times.Once);
    }
}
