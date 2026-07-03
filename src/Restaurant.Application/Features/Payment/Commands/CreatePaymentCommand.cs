using FluentValidation;
using MassTransit;
using Restaurant.Application.Features.Orders;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Payment.Models;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Application.Features.Payment.Specifications;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.Waiters;
using Restaurant.Contracts.Events;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Payment.Commands;

public sealed record CreatePaymentCommand(
    long OrderId,
    decimal Amount,
    PaymentMethod Method
) : ICommand<PaymentDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Method).IsInEnum();
    }
}

internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly PaymentMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly TimeProvider _timeProvider;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        PaymentMapper mapper,
        IPublishEndpoint publishEndpoint,
        TimeProvider timeProvider)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _timeProvider = timeProvider;
    }

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new ResourceNotFoundException(OrderErrors.NotFound);

        if (order.WaiterId is null)
            throw new ResourceNotFoundException(WaiterErrors.NotFound);

        var spec = new PaymentByOrderIdSpec(request.OrderId);
        var exists = await _paymentRepository.AnyAsync(spec, cancellationToken);
        if (exists)
            throw new ResourceNotFoundException(PaymentErrors.AlreadyPaid);

        if (request.Amount < order.TotalPrice)
            throw new ResourceNotFoundException(PaymentErrors.InvalidAmount);
        
        var payment = new Domain.Entities.Payment
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Method = request.Method,
            WaiterId = (long)order.WaiterId,
            Status = PaymentStatus.Pending
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new PaymentRequestedEvent
        {
            OrderId = payment.OrderId,
            WaiterId = payment.WaiterId,
            Amount = payment.Amount,
            Message = "Payment Created",
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        }, cancellationToken);

        return _mapper.Map(payment);
    }
}