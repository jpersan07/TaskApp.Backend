using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApp.API.Controllers;
using TaskApp.Application.DTOs;
using TaskApp.Application.Services;

namespace TaskApp.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockService = new Mock<IAuthService>();
        _controller = new AuthController(_mockService.Object);
    }

    [Fact]
    public async Task Register_WhenSucceeds_ReturnsOk()
    {
        var dto = new RegisterDto { Email = "test@test.com", UserName = "testuser", Password = "Pass1!" };
        _mockService.Setup(s => s.Register(dto)).ReturnsAsync(new List<string>());

        var result = await _controller.Register(dto);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Register_WhenFails_ReturnsBadRequest()
    {
        var dto = new RegisterDto { Email = "test@test.com", UserName = "testuser", Password = "Pass1!" };
        var errors = new List<string> { "Error message" };
        _mockService.Setup(s => s.Register(dto)).ReturnsAsync(errors);

        var result = await _controller.Register(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errors, badRequest.Value);
    }

    [Fact]
    public async Task Login_WhenSucceeds_ReturnsOkWithToken()
    {
        var dto = new LoginDto { UserName = "testuser", Password = "Pass1!" };
        var response = new AuthResponseDto { Token = "jwt-token-value" };
        _mockService.Setup(s => s.Login(dto)).ReturnsAsync(response);

        var result = await _controller.Login(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, ok.Value);
    }

    [Fact]
    public async Task Login_WhenFails_ReturnsUnauthorized()
    {
        var dto = new LoginDto { UserName = "testuser", Password = "wrong" };
        _mockService.Setup(s => s.Login(dto)).ReturnsAsync((AuthResponseDto)null!);

        var result = await _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
