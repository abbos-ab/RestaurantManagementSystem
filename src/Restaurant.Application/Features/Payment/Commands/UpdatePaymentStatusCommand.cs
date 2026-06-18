using FluentValidation;
using MediatR;
using Restaurant.Application.Features.Notifications.Events;
using Restaurant.Application.Features.OrderHistories.Events;
using Restaurant.Application.Features.Payment.Models;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Payment.Commands;

public sealed record UpdatePaymentStatusCommand(long PaymentId, PaymentStatus Status) : ICommand<PaymentDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdatePaymentStatusValidator : AbstractValidator<UpdatePaymentStatusCommand>
{
    public UpdatePaymentStatusValidator()
    {
        RuleFor(p => p.PaymentId)
            .GreaterThan(0)
            .WithMessage("Payment Id must be greater than 0");

        RuleFor(p => p.Status).IsInEnum();
    }
}

internal sealed class UpdatePaymentStatusCommandHandler : ICommandHandler<UpdatePaymentStatusCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly PaymentMapper _mapper;
    private readonly IMediator _mediator;

    public UpdatePaymentStatusCommandHandler(IPaymentRepository paymentRepository, PaymentMapper mapper,
        IMediator mediator)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<PaymentDto> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            throw new ResourceNotFoundException(PaymentErrors.NotFound);

        payment.Status = request.Status;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        await _mediator.Publish(new CreateNotificationEvent(
                payment.WaiterId,
                NotificationType.PaymentCompleted,
                payment.OrderId,
                "Order created"),
            cancellationToken
        );

        await _mediator.Publish(new CreateOrderHistoryEvent(
                payment.OrderId,
                OrderHistoryAction.Paid,
                "Order created",
                payment.WaiterId,
                null
            ),
            cancellationToken
        );

        return _mapper.Map(payment);
    }
}