using Microsoft.EntityFrameworkCore;
using TaskApp.Application.Services;
using TaskApp.Domain.Interfaces;
using TaskApp.Infrastructure;
using TaskApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database Connection, always BUILDER - SERVICES - DBCONTEXT - DATABASE TYPE - CONFIG - NAME OF DATABASE RUTE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<IAppTaskService, AppTaskService>();
builder.Services.AddScoped<IAppTaskRepository, AppTaskRepository>();

var app = builder.Build();

app.MapControllers();

app.Run();