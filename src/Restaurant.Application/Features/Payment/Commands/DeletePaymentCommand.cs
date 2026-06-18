using FluentValidation;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Payment.Commands;

public sealed record DeletePaymentCommand(
    long Id
) : ICommand;

// ReSharper disable once UnusedType.Global
public sealed class DeletePaymentCommandValidator : AbstractValidator<DeletePaymentCommand>
{
    public DeletePaymentCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}

internal sealed class DeletePaymentCommandHandler : ICommandHandler<DeletePaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;

    public DeletePaymentCommandHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (payment is null)
            throw new ResourceNotFoundException(PaymentErrors.NotFound);

        await _paymentRepository.DeleteAsync(payment, cancellationToken);

        await _paymentRepository.SaveChangesAsync(cancellationToken);
    }
}