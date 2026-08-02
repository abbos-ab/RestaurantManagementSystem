using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;
using Restaurant.Mediator.Helper.Common.Extensions;

namespace Restaurant.Infrastructure.Persistence.Seeders;

public sealed class CategoryDatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public CategoryDatabaseSeeder(
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task SeedAsync()
    {
        var hasCategories = await _dbContext.Categories.AnyAsync();

        if (hasCategories)
            return;
        
        var createdAt = _timeProvider.GetLocalDateTimeNowKindUtc();
        
        var categories = new List<Category>
        {
            new()
            {
                Name = "Fast Food",
                Description = "Fast food meals including burgers, pizzas and sandwiches",
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            
            new()
            {
                Name = "National Foods",
                Description = "Traditional local dishes and national cuisine",
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            
            new()
            {
                Name = "Soups",
                Description = "Different types of hot soups",
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            
            new()
            {
                Name = "Salads",
                Description = "Fresh vegetable and special salads",
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Name = "Drinks",
                Description = "Cold and hot beverages",
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
                
            new()
            {
                Name = "Desserts",
                Description = "Sweet dishes and desserts",
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            }
        };
        
        await _dbContext.Categories.AddRangeAsync(categories);

        await _dbContext.SaveChangesAsync();
    }
}