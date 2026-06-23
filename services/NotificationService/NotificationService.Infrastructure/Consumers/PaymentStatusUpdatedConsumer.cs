using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Contracts.Events;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Consumers;

public sealed class PaymentStatusUpdatedConsumer : IConsumer<PaymentStatusUpdatedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<PaymentStatusUpdatedConsumer> _logger;

    public PaymentStatusUpdatedConsumer(NotificationDbContext dbContext, ILogger<PaymentStatusUpdatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentStatusUpdatedEvent> context)
    {
        var message = context.Message;

        var notificationType = message.Status == "Paid" 
            ? NotificationType.PaymentCompleted 
            : NotificationType.PaymentRequested;

        var notification = new Notification
        {
            OrderId = message.OrderId,
            UserId = message.WaiterId,
            Type = notificationType,
            Message = $"Payment for Order #{message.OrderId} is now {message.Status}. Processed at {message.UpdatedAt:yyyy-MM-dd HH:mm:ss}.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Notification successfully saved for updated payment status of Order #{OrderId} (Status: {Status}).", 
            message.OrderId, message.Status);
    }
}