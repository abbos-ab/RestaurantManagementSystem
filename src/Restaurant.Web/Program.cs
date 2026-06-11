using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minio;
using Restaurant.Application;
using Restaurant.Application.Services;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Infrastructure.Persistence.Repositories;
using Restaurant.Infrastructure.Services;
using Restaurant.Infrastructure.Settings;
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

builder.Services.AddRepositories();

builder.Services.AddApplicationServices();

builder.Services.AddMinio(x =>
{
    var settings = builder.Configuration
        .GetSection("MinioSettings")
        .Get<MinioSettings>();

    x.WithEndpoint(settings.Endpoint);
    x.WithCredentials(settings.AccessKey, settings.SecretKey);
    x.WithRegion(settings.Region);
    x.WithSSL(settings.Secure);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddJwtConfiguration(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment());

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