using Restaurant.Application.Features.Payment.Models;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Payment.Queries;

public sealed record GetPaymentByIdQuery(
    long Id
) : IQuery<PaymentDto>;

internal sealed class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly PaymentMapper _mapper;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, PaymentMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (payment is null)
            throw new ResourceNotFoundException(PaymentErrors.NotFound);

        return _mapper.Map(payment);
    }
}