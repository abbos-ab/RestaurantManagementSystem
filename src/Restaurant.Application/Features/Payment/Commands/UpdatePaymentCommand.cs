using FluentValidation;
using Restaurant.Application.Features.Payment.Models;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Payment.Commands;

public sealed record UpdatePaymentCommand(
    long PaymentId,
    decimal Amount,
    PaymentMethod Method,
    PaymentStatus Status
) : ICommand<PaymentDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
{
    public UpdatePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithMessage("Payment id most be greater than 0");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount most be greater than 0");

        RuleFor(x => x.Method).IsInEnum();

        RuleFor(x => x.Status).IsInEnum();
    }
}

internal sealed class UpdatePaymentCommandHandler : ICommandHandler<UpdatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly PaymentMapper _mapper;

    public UpdatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        PaymentMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
            throw new ResourceNotFoundException(PaymentErrors.NotFound);

        payment.Amount = request.Amount;
        payment.Method = request.Method;
        payment.Status = request.Status;

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map(payment);
    }
}