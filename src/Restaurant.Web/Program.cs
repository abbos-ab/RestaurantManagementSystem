using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Mediator.Helper.Middlewares;
using Restaurant.Web;
using Restaurant.Web.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddHostedService<HangfireJobRegistrationService>();
builder.Services.AddHostedService<MinioBucketBackgroundService>();

builder.Services.AddDataProtection();

builder.Services.AddScoped<CurrentUserMiddleware>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", document),
                new List<string>()
            }
        });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment())
    .AddJwtConfiguration(builder.Configuration);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<ExceptionToProblemDetailsHandler>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

app.MapControllers();

app.Run();

//TODO telegram Send Exception
//TODO Logging
//TODO ElasticSearch
