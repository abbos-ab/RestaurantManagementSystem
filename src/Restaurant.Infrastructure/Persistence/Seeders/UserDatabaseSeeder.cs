using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;
using Restaurant.Mediator.Helper.Common.Extensions;

namespace Restaurant.Infrastructure.Persistence.Seeders;

public sealed class UserDatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public UserDatabaseSeeder(
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task SeedAsync()
    {
        var hasUsers = await _dbContext.Users.AnyAsync();

        if (hasUsers)
            return;

        var createdAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        var users = new List<User>
        {
            new()
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@restaurant.local",
                PhoneNumber = PhoneNumber.Create("+992900000001"),
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                FirstName = "John",
                LastName = "Waiter",
                Email = "waiter1@restaurant.local",
                PhoneNumber = PhoneNumber.Create("+992900000002"),
                Password = BCrypt.Net.BCrypt.HashPassword("Waiter123!"),
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                FirstName = "Ali",
                LastName = "Waiter",
                Email = "waiter2@restaurant.local",
                PhoneNumber = PhoneNumber.Create("+992900000003"),
                Password = BCrypt.Net.BCrypt.HashPassword("Waiter123!"),
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                FirstName = "Akmal",
                LastName = "Chef",
                Email = "chef1@restaurant.local",
                PhoneNumber = PhoneNumber.Create("+992900000004"),
                Password = BCrypt.Net.BCrypt.HashPassword("Chef123!"),
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                FirstName = "Jasur",
                LastName = "Chef",
                Email = "chef2@restaurant.local",
                PhoneNumber = PhoneNumber.Create("+992900000005"),
                Password = BCrypt.Net.BCrypt.HashPassword("Chef123!"),
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            }
        };

        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();
    }
}