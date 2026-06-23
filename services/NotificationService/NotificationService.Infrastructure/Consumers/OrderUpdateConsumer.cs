using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;
using Restaurant.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers;

public class OrderUpdateConsumer : IConsumer<OrderUpdatedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<OrderUpdateConsumer> _logger;

    public OrderUpdateConsumer(NotificationDbContext dbContext, ILogger<OrderUpdateConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderUpdatedEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            OrderId = message.OrderId,
            UserId = message.UserId,
            Type = NotificationType.OrderUpdated,
            Message = $"Order #{message.OrderId} has been updated. New total: {message.TotalAmount} USD.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Notification successfully saved for OrderId: {OrderId}.", message.OrderId);
    }
}