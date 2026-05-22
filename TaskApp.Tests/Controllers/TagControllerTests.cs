using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApp.API.Controllers;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.Tests.Controllers;

public class TagControllerTests
{
    private readonly Mock<ITagService> _mockService;
    private readonly TagController _controller;

    public TagControllerTests()
    {
        _mockService = new Mock<ITagService>();
        _controller = new TagController(_mockService.Object);
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

    private static TagDto BuildDto(int id = 1) => new TagDto
    {
        Id = id,
        Name = "Tag " + id,
        UserId = "user1"
    };

    [Fact]
    public async Task GetAll_ReturnsOkWithTagList()
    {
        var tags = new List<TagDto> { BuildDto(1), BuildDto(2) };
        _mockService.Setup(s => s.GetAllTags("user1")).ReturnsAsync(tags);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tags, ok.Value);
    }

    [Fact]
    public async Task GetAll_CallsServiceWithCorrectUserId()
    {
        _mockService.Setup(s => s.GetAllTags("user1")).ReturnsAsync(new List<TagDto>());

        await _controller.GetAll();

        _mockService.Verify(s => s.GetAllTags("user1"), Times.Once);
    }

    [Fact]
    public async Task GetTagById_WhenFound_ReturnsOkWithDto()
    {
        var dto = BuildDto(1);
        _mockService.Setup(s => s.GetTagById(1)).ReturnsAsync(dto);

        var result = await _controller.GetTagById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetTagById_WhenNotFound_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetTagById(99)).ReturnsAsync((TagDto?)null);

        var result = await _controller.GetTagById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateTag_ReturnsCreatedAtActionWithCorrectIdAndDto()
    {
        var createDto = new CreateTagDto { Name = "Bug" };
        var createdDto = BuildDto(4);
        _mockService.Setup(s => s.CreateTag(createDto, "user1")).ReturnsAsync(createdDto);

        var result = await _controller.CreateTag(createDto);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetTagById), createdAt.ActionName);
        Assert.Equal(4, createdAt.RouteValues!["id"]);
        Assert.Equal(createdDto, createdAt.Value);
    }

    [Fact]
    public async Task CreateTag_CallsServiceWithCorrectUserIdAndDto()
    {
        var createDto = new CreateTagDto { Name = "Feature" };
        _mockService.Setup(s => s.CreateTag(createDto, "user1")).ReturnsAsync(BuildDto(1));

        await _controller.CreateTag(createDto);

        _mockService.Verify(s => s.CreateTag(createDto, "user1"), Times.Once);
    }

    [Fact]
    public async Task UpdateTag_ReturnsNoContent()
    {
        var uptDto = new UpdateTagDto { Name = "Updated" };

        var result = await _controller.UpdateTag(uptDto, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTag_CallsServiceWithCorrectIdAndDto()
    {
        var uptDto = new UpdateTagDto { Name = "Updated" };

        await _controller.UpdateTag(uptDto, 8);

        _mockService.Verify(s => s.UpdateTag(uptDto, 8), Times.Once);
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
        await _controller.DeleteById(6);

        _mockService.Verify(s => s.DeleteTag(6), Times.Once);
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
