using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Contracts.Events;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Consumers;

public sealed class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        NotificationDbContext dbContext, 
        ILogger<OrderPlacedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;

        if (message.OrderId <= 0)
        {
            _logger.LogError("Invalid order data received. OrderId: {OrderId}", message.OrderId);
            return;
        }

        var notification = new Notification
        {
            OrderId = message.OrderId,
            UserId = message.UserId,
            Type = NotificationType.OrderCreated,
            Message = $"Dear {message.CustomerName}, your order has been successfully placed! Total amount: {message.TotalAmount:N0} UZS.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Notification successfully saved for OrderId: {OrderId}.", message.OrderId);
    }
}