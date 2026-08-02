using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;
using Restaurant.Mediator.Helper.Common.Extensions;

namespace Restaurant.Infrastructure.Persistence.Seeders;

public sealed class DishDatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public DishDatabaseSeeder(
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task SeedAsync()
    {
        var hasDishes = await _dbContext.Dishes.AnyAsync();

        if (hasDishes)
            return;

        var createdAt = _timeProvider.GetLocalDateTimeNowKindUtc();
        
        var fastFood = await _dbContext.Categories
            .FirstAsync(x => x.Name == "Fast Food");
        
        var national = await _dbContext.Categories
            .FirstAsync(x => x.Name == "National Foods");

        var soups = await _dbContext.Categories
            .FirstAsync(x => x.Name == "Soups");
        
        var drinks = await _dbContext.Categories
            .FirstAsync(x => x.Name == "Drinks");
        
        var desserts = await _dbContext.Categories
            .FirstAsync(x => x.Name == "Desserts");
        
        var dishes = new List<Dish>
        {
            new()
            {
                Name = "Burger",
                Description = "Classic beef burger",
                Price = 45,
                CategoryId = fastFood.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Pizza Margherita",
                Description = "Italian pizza with cheese and tomato",
                Price = 80,
                CategoryId = fastFood.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Lavash",
                Description = "Chicken lavash",
                Price = 35,
                CategoryId = fastFood.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Osh",
                Description = "Traditional Uzbek plov",
                Price = 60,
                CategoryId = national.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Shashlik",
                Description = "Grilled meat skewers",
                Price = 70,
                CategoryId = national.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Lagman",
                Description = "Traditional noodles",
                Price = 55,
                CategoryId = national.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Chicken Soup",
                Description = "Hot chicken soup",
                Price = 40,
                CategoryId = soups.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Caesar Salad",
                Description = "Fresh Caesar salad",
                Price = 50,
                CategoryId = national.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Coca Cola",
                Description = "Cold drink",
                Price = 10,
                CategoryId = drinks.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Green Tea",
                Description = "Hot green tea",
                Price = 8,
                CategoryId = drinks.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Ice Cream",
                Description = "Vanilla ice cream",
                Price = 25,
                CategoryId = desserts.Id,
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            }
        };
        
        await _dbContext.Dishes.AddRangeAsync(dishes);

        await _dbContext.SaveChangesAsync();
    }
}