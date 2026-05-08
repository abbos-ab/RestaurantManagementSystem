using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Orders.Repositories;

namespace Restaurant.Infrastructure.Persistence.Repositories;

public static class ConfigureServices
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDishRepository, DishRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        
        return services;
    }
}