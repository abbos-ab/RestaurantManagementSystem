using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;
using Restaurant.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers;

public sealed class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(NotificationDbContext dbContext, ILogger<OrderCancelledConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            OrderId = message.OrderId,
            UserId = message.UserId,
            Type = NotificationType.OrderCancelled,
            Message = $"Dear customer, your order #{message.OrderId} has been cancelled. Reason: {message.Reason}.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Order #{OrderId} cancellation notification saved.", message.OrderId);
    }
}