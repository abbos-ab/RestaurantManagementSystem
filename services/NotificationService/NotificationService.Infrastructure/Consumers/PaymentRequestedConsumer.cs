using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Contracts.Events;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Consumers;

public sealed class PaymentRequestedConsumer : IConsumer<PaymentRequestedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<PaymentRequestedConsumer> _logger;

    public PaymentRequestedConsumer(
        NotificationDbContext dbContext,
        ILogger<PaymentRequestedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentRequestedEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            OrderId = message.OrderId,
            UserId = message.WaiterId,
            Type = NotificationType.PaymentRequested,
            Message = $"Payment request initiated for Order #{message.OrderId}. Total Amount: {message.Amount}.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Notification successfully created for payment request on Order #{OrderId}",
            message.OrderId);
    }
}