using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Carts.Repositories;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Infrastructure.Persistence.Repositories;

public static class ConfigureServices
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        
        services.AddScoped<IDishRepository, DishRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
        services.AddScoped<ITableRepository, TableRepository>();    
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IDishMediaRepository, DishMediaRepository>();
        services.AddScoped<IDishMediaRelationRepository, DishMediaRelationRepository>();
        services.AddScoped<IReviewRepository, ReviewRepositories>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        return services;
    }
}