using FluentValidation;
using NotificationService.Application.Features.Notifications.Models;
using NotificationService.Application.Features.Notifications.Repositories;
using NotificationService.Application.Features.Notifications.Specifications;
using NotificationService.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace NotificationService.Application.Features.Notifications.Commands;

public sealed record UpdateNotificationCommand(
    long Id,
    NotificationType Type,
    string? Message
) : ICommand<NotificationDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateNotificationCommandValidator
    : AbstractValidator<UpdateNotificationCommand>
{
    public UpdateNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Message)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Message));
    }
}

internal sealed class UpdateNotificationCommandHandler : ICommandHandler<UpdateNotificationCommand, NotificationDto>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationMapper _notificationMapper;

    public UpdateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        NotificationMapper notificationMapper,
        TimeProvider timeProvider)
    {
        _notificationRepository = notificationRepository;
        _notificationMapper = notificationMapper;
    }

    public async Task<NotificationDto> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FirstOrDefaultAsync(
            new NotificationByIdSpec(request.Id),
            cancellationToken);

        if (notification is null) throw new BusinessLogicException(NotificationErrors.NotFound);

        notification.Type = request.Type;
        notification.Message = request.Message;

        await _notificationRepository.UpdateAsync(notification, cancellationToken);

        return _notificationMapper.Map(notification);
    }
}