using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskApp.Application.DTOs;
using TaskApp.Domain.Entities;
using TaskApp.Infrastructure.Services;

namespace TaskApp.Tests.Infrastructure;

public class AuthServiceTests
{
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _mockSignInManager = new Mock<SignInManager<AppUser>>(
            _mockUserManager.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
            null!, null!, null!, null!);

        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("super-secret-key-for-testing-1234567890");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _service = new AuthService(_mockUserManager.Object, _mockSignInManager.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task Register_WhenSucceeds_ReturnsTrue()
    {
        var dto = new RegisterDto { Email = "test@test.com", UserName = "testuser", Password = "Pass1!" };
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _service.Register(dto);

        Assert.True(result);
    }

    [Fact]
    public async Task Register_WhenFails_ReturnsFalse()
    {
        var dto = new RegisterDto { Email = "bad@test.com", UserName = "bad", Password = "weak" };
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var result = await _service.Register(dto);

        Assert.False(result);
    }

    [Fact]
    public async Task Register_WhenThrows_WrapsException()
    {
        var dto = new RegisterDto { Email = "x@x.com", UserName = "x", Password = "x" };
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), dto.Password))
            .ThrowsAsync(new Exception("DB down"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.Register(dto));
        Assert.Contains("Error registering", ex.Message);
    }

    [Fact]
    public async Task Login_WhenUserNotFound_ReturnsNull()
    {
        var dto = new LoginDto { UserName = "ghost", Password = "pass" };
        _mockUserManager.Setup(m => m.FindByNameAsync("ghost")).ReturnsAsync((AppUser?)null);

        var result = await _service.Login(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_WhenWrongPassword_ReturnsNull()
    {
        var user = new AppUser { Id = "u1", UserName = "testuser" };
        var dto = new LoginDto { UserName = "testuser", Password = "wrong" };
        _mockUserManager.Setup(m => m.FindByNameAsync("testuser")).ReturnsAsync(user);
        _mockSignInManager
            .Setup(m => m.CheckPasswordSignInAsync(user, "wrong", false))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _service.Login(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_WhenSucceeds_ReturnsTokenDto()
    {
        var user = new AppUser { Id = "u1", UserName = "testuser" };
        var dto = new LoginDto { UserName = "testuser", Password = "Pass1!" };
        _mockUserManager.Setup(m => m.FindByNameAsync("testuser")).ReturnsAsync(user);
        _mockSignInManager
            .Setup(m => m.CheckPasswordSignInAsync(user, "Pass1!", false))
            .ReturnsAsync(SignInResult.Success);

        var result = await _service.Login(dto);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public async Task Login_WhenThrows_WrapsException()
    {
        var dto = new LoginDto { UserName = "crash", Password = "pass" };
        _mockUserManager.Setup(m => m.FindByNameAsync("crash")).ThrowsAsync(new Exception("DB error"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.Login(dto));
        Assert.Contains("Error logging in", ex.Message);
    }
}
