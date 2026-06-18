using FluentValidation;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Events;

namespace Restaurant.Application.Features.Notifications.Events;

public sealed record CreateNotificationEvent(
    long? UserId,
    NotificationType Type,
    long OrderId,
    string? Message
) : IEvent;

// ReSharper disable once UnusedType.Global
public sealed class CreateNotificationEventValidator : AbstractValidator<CreateNotificationEvent>
{
    public CreateNotificationEventValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Message)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Message));
    }
}

internal sealed class CreateNotificationEventHandler : IEventHandler<CreateNotificationEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly TimeProvider _timeProvider;

    public CreateNotificationEventHandler(
        INotificationRepository notificationRepository,
        NotificationMapper notificationMapper,
        TimeProvider timeProvider)
    {
        _notificationRepository = notificationRepository;
        _timeProvider = timeProvider;
    }

    public async Task Handle(CreateNotificationEvent request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            UserId = request.UserId,
            Type = request.Type,
            OrderId = request.OrderId,
            Message = request.Message,
            IsRead = false,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
            UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
    }
}