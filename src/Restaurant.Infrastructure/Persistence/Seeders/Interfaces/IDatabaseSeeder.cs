namespace Restaurant.Infrastructure.Persistence.Seeders.Interfaces;

public interface IDatabaseSeeder
{
    Task SeedAsync();
}