using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;
using TaskApp.Infrastructure;
using TaskApp.Infrastructure.Repositories;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Tests.Infrastructure;

public class AppTaskRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static AppUser SeedUser(AppDbContext ctx, string id = "user1")
    {
        var user = new AppUser { Id = id, UserName = id };
        ctx.Users.Add(user);
        ctx.SaveChanges();
        return user;
    }

    private static AppTask BuildTask(string userId = "user1", int? categoryId = null) => new()
    {
        Title = "Task",
        UserId = userId,
        CategoryId = categoryId,
        Status = TaskStatus.NonStarted,
        SubTasks = [],
        Tags = []
    };

    [Fact]
    public async Task GetAllByUserId_ReturnsOnlyTasksForThatUser()
    {
        using var ctx = CreateContext();
        SeedUser(ctx, "user1");
        SeedUser(ctx, "user2");
        ctx.AppTasks.Add(BuildTask("user1"));
        ctx.AppTasks.Add(BuildTask("user2"));
        await ctx.SaveChangesAsync();
        var repo = new AppTaskRepository(ctx);

        var result = await repo.GetAllByUserId("user1");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetTaskById_WhenExists_ReturnsTask()
    {
        using var ctx = CreateContext();
        SeedUser(ctx);
        var task = BuildTask();
        ctx.AppTasks.Add(task);
        await ctx.SaveChangesAsync();
        var repo = new AppTaskRepository(ctx);

        var result = await repo.GetTaskById(task.Id);

        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public async Task GetTaskById_WhenNotExists_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new AppTaskRepository(ctx);

        var result = await repo.GetTaskById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllByUserId_TaskWithCategoryId_MapsCorrectly()
    {
        using var ctx = CreateContext();
        SeedUser(ctx);
        var task = BuildTask("user1", categoryId: 5);
        ctx.AppTasks.Add(task);
        await ctx.SaveChangesAsync();
        var repo = new AppTaskRepository(ctx);

        var result = (await repo.GetAllByUserId("user1")).First();

        Assert.Equal(5, result.CategoryId);
    }
}

public class CategoryRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task GetAllByUserId_ReturnsOnlyCategoriesForThatUser()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(new Category { Name = "Work", UserId = "user1" });
        ctx.Categories.Add(new Category { Name = "Sport", UserId = "user2" });
        await ctx.SaveChangesAsync();
        var repo = new CategoryRepository(ctx);

        var result = await repo.GetAllByUserId("user1");

        Assert.Single(result);
        Assert.Equal("Work", result.First().Name);
    }
}

public class SubTaskRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task GetAllByTaskId_ReturnsOnlySubTasksForThatTask()
    {
        using var ctx = CreateContext();
        var task = new AppTask { Title = "Parent", UserId = "user1", SubTasks = [], Tags = [] };
        ctx.AppTasks.Add(task);
        await ctx.SaveChangesAsync();

        ctx.SubTasks.Add(new SubTask { Title = "Sub1", TaskId = task.Id });
        ctx.SubTasks.Add(new SubTask { Title = "Sub2", TaskId = task.Id });
        ctx.SubTasks.Add(new SubTask { Title = "Other", TaskId = 999 });
        await ctx.SaveChangesAsync();
        var repo = new SubTaskRepository(ctx);

        var result = await repo.GetAllByTaskId(task.Id);

        Assert.Equal(2, result.Count());
    }
}

public class TagRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task GetAllByUserId_ReturnsOnlyTagsForThatUser()
    {
        using var ctx = CreateContext();
        ctx.Tags.Add(new Tag { Name = "Urgent", UserId = "user1" });
        ctx.Tags.Add(new Tag { Name = "Low", UserId = "user2" });
        await ctx.SaveChangesAsync();
        var repo = new TagRepository(ctx);

        var result = await repo.GetAllByUserId("user1");

        Assert.Single(result);
        Assert.Equal("Urgent", result.First().Name);
    }
}
