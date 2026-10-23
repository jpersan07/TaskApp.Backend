using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskApp.Domain.Interfaces;

namespace TaskApp.API.Middleware;

public class TaskOwnershipMiddleware
{
  private readonly RequestDelegate _next;
  private readonly IServiceScopeFactory _scopeFactory;
  public TaskOwnershipMiddleware(RequestDelegate next, IServiceScopeFactory repository)
  {
    _next = next;
    _scopeFactory = repository;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    using var scope = _scopeFactory.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<IAppTaskRepository>();

    string taskString = Convert.ToString(context.Request.RouteValues["id"])!;

    if (string.IsNullOrEmpty(taskString))
    {
      await _next(context);
      return;
    }

    int taskId = int.Parse(taskString);
    string userId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

    var task = await repo.GetTaskById(taskId);

    if (task == null)
    {
      context.Response.StatusCode = 404;
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync($"{{\"statusCode\": 404, \"message\": \"Cant find that Task\"}}");
      return;
    }

    if (task.UserId == userId)
    {
      await _next(context);
      return;
    }
    else
    {
      context.Response.StatusCode = 403;
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync($"{{\"statusCode\": 403, \"message\": \"Unauthorized\"}}");
    }


  }
}
