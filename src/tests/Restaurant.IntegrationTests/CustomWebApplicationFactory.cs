using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Restaurant.Infrastructure.Persistence;
using Restaurant.IntegrationTests.Auth;
using Testcontainers.PostgreSql;

namespace Restaurant.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder()
            .WithDatabase("restaurant")
            .WithUsername("postgres")
            .WithPassword("123456")
            .Build();

    private readonly GitHubApiServer _gitHubApiServer = new();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_container.GetConnectionString());
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme,
                    _ => { });
        });
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync();
        
        _gitHubApiServer.Start();
        _gitHubApiServer.SetupUser("validUSer");
    }

    public async Task DisposeAsync()
    {
        _gitHubApiServer.Dispose();
        await _container.DisposeAsync();
    }
}