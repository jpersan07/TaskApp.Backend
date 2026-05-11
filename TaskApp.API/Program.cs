using Microsoft.EntityFrameworkCore;
using TaskApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Database Connection, always BUILDER - SERVICES - DBCONTEXT - DATABASE TYPE - CONFIG - NAME OF DATABASE RUTE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.Run();