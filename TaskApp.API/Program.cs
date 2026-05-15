using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskApp.Application.Services;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Interfaces;
using TaskApp.Infrastructure;
using TaskApp.Infrastructure.Repositories;
using TaskApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Database Connection, always BUILDER - SERVICES - DBCONTEXT - DATABASE TYPE - CONFIG - NAME OF DATABASE RUTE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<IAppTaskService, AppTaskService>();
builder.Services.AddScoped<IAppTaskRepository, AppTaskRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ISubTaskService, SubTaskService>();
builder.Services.AddScoped<ISubTaskRepository, SubTaskRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(options =>{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new
    SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
  };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();