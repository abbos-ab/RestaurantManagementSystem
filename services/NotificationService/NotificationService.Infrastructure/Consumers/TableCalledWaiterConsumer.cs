using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Contracts.Events;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Consumers;

public sealed class TableCalledWaiterConsumer : IConsumer<TableCalledWaiterEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<TableCalledWaiterConsumer> _logger;

    public TableCalledWaiterConsumer(NotificationDbContext dbContext, ILogger<TableCalledWaiterConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TableCalledWaiterEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            UserId = message.WaiterId,
            OrderId = null,
            Type = NotificationType.TableCalledWaiter,
            Message = $"Table #{message.TableId} is requesting assistance.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Notification successfully dispatched for Waiter #{WaiterId} from Table #{TableId}.",
            message.WaiterId, message.TableId);
    }
}