using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaskApp.API.Middleware;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Tests.Middleware;

public class TaskOwnershipMiddlewareTests
{
    private static (TaskOwnershipMiddleware middleware, Mock<IAppTaskRepository> mockRepo) Build(RequestDelegate next)
    {
        var mockRepo = new Mock<IAppTaskRepository>();

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IAppTaskRepository)))
            .Returns(mockRepo.Object);

        var mockScope = new Mock<IServiceScope>();
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

        return (new TaskOwnershipMiddleware(next, mockScopeFactory.Object), mockRepo);
    }

    private static DefaultHttpContext BuildContext(string? routeId, string userId)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        if (routeId != null)
            context.Request.RouteValues["id"] = routeId;

        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userId) };
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        return context;
    }

    private static AppTask BuildTask(int id, string userId) => new()
    {
        Id = id,
        Title = "Test",
        UserId = userId,
        Status = TaskStatus.NonStarted,
        User = new AppUser { UserName = "testuser" },
        SubTasks = [],
        Tags = []
    };

    [Fact]
    public async Task InvokeAsync_WhenNoRouteId_CallsNext()
    {
        var nextCalled = false;
        var (middleware, _) = Build(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = BuildContext(null, "user1");

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WhenTaskNotFound_Returns404()
    {
        var (middleware, mockRepo) = Build(_ => Task.CompletedTask);
        mockRepo.Setup(r => r.GetTaskById(99)).ReturnsAsync((AppTask?)null);
        var context = BuildContext("99", "user1");

        await middleware.InvokeAsync(context);

        Assert.Equal(404, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("404", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenTaskBelongsToUser_CallsNext()
    {
        var nextCalled = false;
        var (middleware, mockRepo) = Build(_ => { nextCalled = true; return Task.CompletedTask; });
        mockRepo.Setup(r => r.GetTaskById(1)).ReturnsAsync(BuildTask(1, "user1"));
        var context = BuildContext("1", "user1");

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WhenTaskBelongsToDifferentUser_Returns403()
    {
        var (middleware, mockRepo) = Build(_ => Task.CompletedTask);
        mockRepo.Setup(r => r.GetTaskById(1)).ReturnsAsync(BuildTask(1, "owner"));
        var context = BuildContext("1", "attacker");

        await middleware.InvokeAsync(context);

        Assert.Equal(403, context.Response.StatusCode);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("403", body);
    }
}
