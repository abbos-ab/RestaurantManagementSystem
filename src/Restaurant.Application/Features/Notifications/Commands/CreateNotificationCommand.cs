using FluentValidation;
using Restaurant.Application.Features.Notifications.Models;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.Notifications.Commands;

public sealed record CreateNotificationCommand(
    long UserId,
    NotificationType Type,
    long? OrderId,
    string? Message
) : ICommand<NotificationDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Message)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Message));
    }
}

internal sealed class CreateNotificationCommandHandler : ICommandHandler<CreateNotificationCommand, NotificationDto>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationMapper _notificationMapper;
    private readonly TimeProvider _timeProvider;

    public CreateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        NotificationMapper notificationMapper,
        TimeProvider timeProvider)
    {
        _notificationRepository = notificationRepository;
        _notificationMapper = notificationMapper;
        _timeProvider = timeProvider;
    }

    public async Task<NotificationDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
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

        return _notificationMapper.Map(notification);
    }
}