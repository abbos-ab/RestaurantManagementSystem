using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Infrastructure.Persistence.Repositories;
using Restaurant.Web;
using Restaurant.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddHostedService<HangfireJobRegistrationService>();
builder.Services.AddHostedService<MinioBucketBackgroundService>();

builder.Services.AddDataProtection();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddApplicationServices()
    .AddJwtConfiguration(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();