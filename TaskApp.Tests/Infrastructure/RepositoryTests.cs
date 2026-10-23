using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using TaskApp.Domain.Entities;
using TaskApp.Infrastructure;
using TaskApp.Infrastructure.Repositories;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Tests.Infrastructure;

internal sealed class FailingDbContext : AppDbContext
{
    public FailingDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => throw new DbUpdateException("Forced failure for testing");
}

public class RepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static FailingDbContext CreateFailingContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FailingDbContext(options);
    }

    private static AppTask BuildTask(string userId = "user1") => new()
    {
        Title = "Test Task",
        Description = "Desc",
        UserId = userId,
        Status = TaskStatus.NonStarted,
        CategoryId = null,
        SubTasks = [],
        Tags = []
    };

    [Fact]
    public async Task Add_PersistsEntityToDatabase()
    {
        using var ctx = CreateContext();
        var repo = new Repository<AppTask>(ctx);
        var task = BuildTask();

        await repo.Add(task);

        Assert.Equal(1, await ctx.AppTasks.CountAsync());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsEntity()
    {
        using var ctx = CreateContext();
        var repo = new Repository<AppTask>(ctx);
        var task = BuildTask();
        await repo.Add(task);

        var result = await repo.GetById(task.Id);

        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new Repository<AppTask>(ctx);

        var result = await repo.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_ReturnsAllEntities()
    {
        using var ctx = CreateContext();
        var repo = new Repository<AppTask>(ctx);
        await repo.Add(BuildTask("u1"));
        await repo.Add(BuildTask("u2"));

        var result = await repo.GetAll();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Update_PersistsChanges()
    {
        using var ctx = CreateContext();
        var repo = new Repository<AppTask>(ctx);
        var task = BuildTask();
        await repo.Add(task);

        task.Title = "Updated Title";
        await repo.Update(task);

        var updated = await ctx.AppTasks.FindAsync(task.Id);
        Assert.Equal("Updated Title", updated!.Title);
    }

    [Fact]
    public async Task Delete_WhenEntityExists_RemovesIt()
    {
        using var ctx = CreateContext();
        var repo = new Repository<AppTask>(ctx);
        var task = BuildTask();
        await repo.Add(task);

        await repo.Delete(task.Id);

        Assert.Equal(0, await ctx.AppTasks.CountAsync());
    }

    [Fact]
    public async Task Delete_WhenEntityNotExists_DoesNotThrow()
    {
        using var ctx = CreateContext();
        var repo = new Repository<AppTask>(ctx);

        await repo.Delete(999);

        Assert.Equal(0, await ctx.AppTasks.CountAsync());
    }

    [Fact]
    public async Task Add_WhenSaveChangesFails_ThrowsWrappedException()
    {
        using var ctx = CreateFailingContext();
        var repo = new Repository<AppTask>(ctx);

        await Assert.ThrowsAsync<DbUpdateException>(() => repo.Add(BuildTask()));
    }

    [Fact]
    public async Task Update_WhenSaveChangesFails_ThrowsWrappedException()
    {
        var normalOpts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        AppTask task;
        await using (var seedCtx = new AppDbContext(normalOpts))
        {
            task = BuildTask();
            seedCtx.AppTasks.Add(task);
            await seedCtx.SaveChangesAsync();
        }

        await using var failCtx = new FailingDbContext(normalOpts);
        var repo = new Repository<AppTask>(failCtx);
        await Assert.ThrowsAsync<DbUpdateException>(() => repo.Update(task));
    }

    [Fact]
    public async Task Delete_WhenEntityExistsAndSaveChangesFails_ThrowsWrappedException()
    {
        var normalOpts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        AppTask task;
        await using (var seedCtx = new AppDbContext(normalOpts))
        {
            task = BuildTask();
            seedCtx.AppTasks.Add(task);
            await seedCtx.SaveChangesAsync();
        }

        await using var failCtx = new FailingDbContext(normalOpts);
        var repo = new Repository<AppTask>(failCtx);
        await Assert.ThrowsAsync<DbUpdateException>(() => repo.Delete(task.Id));
    }
}
